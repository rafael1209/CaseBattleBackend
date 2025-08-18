using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CaseBattleBackend.Repositories;

public class CaseRepository(IMongoDbContext context) : ICaseRepository
{
    private readonly IMongoCollection<Case> _cases = context.CasesCollection;

    public async Task<Case> Create(Case newCase)
    {
        await _cases.InsertOneAsync(newCase);

        return newCase;
    }

    public async Task<List<Case>> GetAll(int page = 1, int pageSize = 15)
    {
        var skip = (page - 1) * pageSize;

        var pipeline = new[]
        {
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "gameResults" },
                { "localField", "_id" },
                { "foreignField", "caseId" },
                { "as", "results" }
            }),
            new BsonDocument("$addFields", new BsonDocument { { "openedCount", new BsonDocument("$size", "$results") } }),
            new BsonDocument("$project", new BsonDocument { { "results", 0 } }),
            new BsonDocument("$sort", new BsonDocument("openedCount", -1))
        };

        var allCases = await _cases
            .Aggregate()
            .AppendStage<Case>(pipeline[0])
            .AppendStage<Case>(pipeline[1])
            .AppendStage<Case>(pipeline[2])
            .AppendStage<Case>(pipeline[3])
            .ToListAsync();

        var mostPopular = allCases.Take(4).ToList();
        var leastPopular = allCases.Skip(Math.Max(0, allCases.Count - 4)).ToList();
        var others = allCases.Except(mostPopular).Except(leastPopular).ToList();

        var orderedCases = mostPopular.Concat(leastPopular).Concat(others).ToList();

        return orderedCases.Skip(skip).Take(pageSize).ToList();
    }

    public async Task<Case?> GetById(ObjectId id)
    {
        var filter = Builders<Case>.Filter.Eq(u => u.Id, id);

        return await _cases.Find(filter).FirstOrDefaultAsync();
    }
}