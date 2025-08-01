using CaseBattleBackend.Dtos;
using CaseBattleBackend.Enums;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Services;

public class GameResultService(
    IGameResultRepository gameResultRepository,
    IMinecraftAssets minecraftAssets,
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
                ImageUrl = null,
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
}