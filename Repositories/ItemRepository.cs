using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CaseBattleBackend.Repositories;

public class ItemRepository(IMongoDbContext context) : IItemRepository
{
    private readonly IMongoCollection<CaseItem> _users = context.ItemsCollection;

    public async Task<CaseItem> Create(CaseItem item)
    {
        await _users.InsertOneAsync(item);

        return item;
    }
}