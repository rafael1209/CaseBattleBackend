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

    public async Task<CaseItem?> GetById(ObjectId id)
    {
        var filter = Builders<CaseItem>.Filter.Eq(u => u.Id, id);

        return await _items.Find(filter).FirstOrDefaultAsync();
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

    public async Task<List<CaseItem>> Get(int fromPrice = 0, int page = 1, int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize is < 1 or > 20) pageSize = 20;

        return await _items
            .Find(i => i.Price >= fromPrice)
            .SortBy(i => i.Price)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }
}