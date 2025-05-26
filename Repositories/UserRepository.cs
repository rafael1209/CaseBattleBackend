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
}