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

    public async Task<List<CaseItem>> Get()
    {
        return await _items
            .Find(_ => true)
            .SortBy(i => i.Price)
            .ToListAsync();
    }

    public async Task AddToInventory(ObjectId userId, List<InventoryItem> items)
    {
        var userFilter = Builders<User>.Filter.Eq(u => u.Id, userId);
        var update = Builders<User>.Update.PushEach(u => u.Items, items);
        var result = await context.UsersCollection.UpdateOneAsync(userFilter, update);

        if (result.ModifiedCount == 0)
            throw new Exception("Failed to add items to inventory.");
    }
}