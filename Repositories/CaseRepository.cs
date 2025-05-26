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

    public async Task<List<Case>> GetAll()
    {
        var cases = await _cases.FindAsync(_ => true);

        return await cases.ToListAsync();
    }

    public async Task<Case?> GetById(ObjectId id)
    {
        var filter = Builders<Case>.Filter.Eq(u => u.Id, id);

        return await _cases.Find(filter).FirstOrDefaultAsync();
    }
}