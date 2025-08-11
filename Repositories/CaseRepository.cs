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

        var casesCursor = await _cases.FindAsync(_ => true, new FindOptions<Case>
        { Skip = skip, Limit = pageSize });

        return await casesCursor.ToListAsync();
    }

    public async Task<Case?> GetById(ObjectId id)
    {
        var filter = Builders<Case>.Filter.Eq(u => u.Id, id);

        return await _cases.Find(filter).FirstOrDefaultAsync();
    }
}