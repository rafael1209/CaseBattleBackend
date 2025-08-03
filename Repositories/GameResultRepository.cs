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

    public async Task<List<GameResult>> GetGameHistory(int limit = 12)
    {
        var filter = Builders<GameResult>.Filter.Empty;
        var sort = Builders<GameResult>.Sort.Descending(x => x.CreatedAt);
        
        return await _result.Find(filter).Sort(sort).Limit(limit).ToListAsync();
    }
}