using CaseBattleBackend.Database;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CaseBattleBackend.Repositories;

public class UserRepository(IMongoDbContext context) : IUserRepository
{
    private readonly IMongoCollection<User> _users = context.UsersCollection;

    public async Task<User?> TryGetByMinecraftUuid(string minecraftUuid)
    {
        var filter = Builders<User>.Filter.Eq(u => u.MinecraftUuid, minecraftUuid);

        return await _users.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<User?> TryGetByAuthToken(string authToken)
    {
        var filter = Builders<User>.Filter.Eq(u => u.AuthToken, authToken);

        return await _users.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<User> Create(User user)
    {
        await _users.InsertOneAsync(user);

        return user;
    }

    public async Task<User?> GetById(ObjectId id)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);

        return await _users.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateBalance(ObjectId id, double amount)
    {
        var filter = Builders<User>.Filter.And(
            Builders<User>.Filter.Eq(u => u.Id, id),
            Builders<User>.Filter.Gte(u => u.Balance, -amount)
        );

        var update = Builders<User>.Update.Inc(u => u.Balance, amount);

        var result = await _users.UpdateOneAsync(filter, update);
        return result.ModifiedCount != 0;
    }

    public async Task<List<InventoryItem>> GetInventoryItems(ObjectId id, int page = 1, int pageSize = 32)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        var user = await _users.Find(filter).FirstOrDefaultAsync();

        var items = user.Items
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return items;
    }

    public async Task AddToInventory(ObjectId userId, List<ObjectId> newItems)
    {
        var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
        if (user == null) return;

        foreach (var itemId in newItems)
        {
            var existingItem = user.Items.FirstOrDefault(i => i.Id == itemId);
            if (existingItem != null)
            {
                existingItem.Amount++;
            }
            else
            {
                user.Items.Add(new InventoryItem(itemId, 1));
            }
        }

        var update = Builders<User>.Update.Set(u => u.Items, user.Items);
        await _users.UpdateOneAsync(u => u.Id == userId, update);
    }
}