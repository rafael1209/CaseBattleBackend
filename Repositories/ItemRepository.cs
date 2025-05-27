using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CaseBattleBackend.Repositories;

public class ItemRepository(IMongoDbContext context) : IItemRepository
{
    private readonly IMongoCollection<CaseItem> _items = context.ItemsCollection;

    public async Task<CaseItem> Create(CaseItem item)
    {
        await _items.InsertOneAsync(item);

        return item;
    }

    public async Task<CaseItem> GetById(ObjectId id)
    {
        var filter = Builders<CaseItem>.Filter.Eq(u => u.Id, id);

        return await _items.Find(filter).FirstAsync();
    }

    public async Task<List<CaseItem>> GetTopByMaxPrice(double maxPrice, int limit)
    {
        var filter = Builders<CaseItem>.Filter.Lte(i => i.Price, maxPrice);

        return await _items
            .Find(filter)
            .SortBy(i => i.Price)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task<List<CaseItem>> Get()
    {
        return await _items
            .Find(_ => true)
            .SortBy(i => i.Price)
            .ToListAsync();
    }
}