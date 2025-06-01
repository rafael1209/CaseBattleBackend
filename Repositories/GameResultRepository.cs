using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Driver;

namespace CaseBattleBackend.Repositories;

public class GameResultRepository(IMongoDbContext context) : IGameResultRepository
{
    private readonly IMongoCollection<GameResult> _result = context.GameResultsCollection;

    public async Task<GameResult> SaveResult(GameResult gameResult)
    {
        await _result.InsertOneAsync(gameResult);

        return gameResult;
    }
}