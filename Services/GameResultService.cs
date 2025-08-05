using CaseBattleBackend.Dtos;
using CaseBattleBackend.Enums;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Services;

public class GameResultService(
    IGameResultRepository gameResultRepository,
    IMinecraftAssets minecraftAssets,
    IStorageService storageService,
    IItemRepository itemRepository,
    WebSocketServerService webSocketServer) : IGameResult
{
    public async Task SaveResult(User user, Case caseData, CaseItemView item, ObjectId gameId)
    {
        var gameResult = new GameResult
        {
            UserId = user.Id,
            ItemId = ObjectId.Parse(item.Id),
            CaseId = caseData.Id,
            Bet = caseData.Price,
            WinMoney = item.Price,
            DropChance = item.PercentChance,
        };

        await gameResultRepository.SaveResult(gameResult);

        webSocketServer.PublishToChannel(SubscriptionChannel.LiveWins, new LiveWin
        {
            User = new UserDto
            {
                Id = user.Id.ToString(),
                Username = user.Username,
                AvatarUrl = await minecraftAssets.GetAvatarUrlById(user.MinecraftUuid),
            },
            Case = new CaseDto
            {
                Id = caseData.Id.ToString(),
                Name = caseData.Name,
                Description = caseData.Description,
                ImageUrl = await storageService.GetFileUrl(caseData.ImageId),
                Price = caseData.Price
            },
            Item = new CaseItemView
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                ImageUrl = item.ImageUrl,
                Amount = item.Amount,
                Price = item.Price,
                PercentChance = item.PercentChance,
                Rarity = item.Rarity
            }
        });
    }

    public async Task<List<LiveWin>> GetLastWins(int limit = 12)
    {
        var gameResults = await gameResultRepository.GetGameHistory(limit);

        var liveWins = new List<LiveWin>();
        foreach (var game in gameResults)
        {
            var item = await itemRepository.GetById(game.ItemId);

            if (item is null)
                continue;

            liveWins.Add(new LiveWin
            {
                Item = new CaseItemView
                {
                    Id = game.ItemId.ToString(),
                    Name = item.Name,
                    Description = item.Description,
                    ImageUrl = item.ImageId != null ? await storageService.GetFileUrl(item.ImageId) : item.MinecraftId != null ? await minecraftAssets.GetItemImageAsync(item.MinecraftId) : null,
                    Amount = item.Amount,
                    Price = item.Price,
                    PercentChance = game.DropChance,
                    Rarity = item.Rarity
                }
            });
        }

        return liveWins;
    }
}