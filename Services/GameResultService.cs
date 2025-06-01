using CaseBattleBackend.Enums;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Services;

public class GameResultService(IGameResultRepository gameResultRepository) : IGameResult
{
    public async Task SaveResult(ObjectId userId, double bet, double winMoney, GameType type, ObjectId gameId)
    {
        if (userId == ObjectId.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        if (bet < 0)
            throw new ArgumentException("Bet amount cannot be negative.", nameof(bet));
        if (winMoney < 0)
            throw new ArgumentException("Win money cannot be negative.", nameof(winMoney));
        if (gameId == ObjectId.Empty)
            throw new ArgumentException("Game ID cannot be empty.", nameof(gameId));
        var gameResult = new GameResult
        {
            UserId = userId,
            Game = new Game()
            {
                Id = gameId,
                Type = type
            },
            Bet = bet,
            WinMoney = winMoney,
        };
        await gameResultRepository.SaveResult(gameResult);
    }
}