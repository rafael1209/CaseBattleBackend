using CaseBattleBackend.Dtos;
using CaseBattleBackend.Enums;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Services;

public class GameResultService(IGameResultRepository gameResultRepository, WebSocketServerService webSocketServer) : IGameResult
{
    public async Task SaveResult(User user, Case caseData, CaseItemViewDto item, GameType type, ObjectId gameId)
    {
        if (gameId == ObjectId.Empty)
            throw new ArgumentException("Game ID cannot be empty.", nameof(gameId));

        var gameResult = new GameResult
        {
            UserId = user.Id,
            Game = new Game()
            {
                Id = gameId,
                Type = type
            },
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
                AvatarUrl = null
            },
            Case = new CaseDto
            {
                Id = caseData.Id.ToString(),
                Name = caseData.Name,
                Description = caseData.Description,
                ImageUrl = null,
                Price = caseData.Price
            },
            Item = item
        });
    }

}