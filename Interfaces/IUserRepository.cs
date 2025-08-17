using CaseBattleBackend.Enums;
using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Interfaces;

public interface IUserRepository
{
    Task<User?> TryGetByMinecraftUuid(string minecraftUuid);
    Task<User?> TryGetByAuthToken(string authToken);
    Task<User?> GetByDiscordId(long discordId);
    Task<User> Create(User user);
    Task<User?> GetById(ObjectId id);
    Task<bool> UpdateBalance(ObjectId id, decimal amount);
    Task<List<InventoryItem>> GetInventoryItems(ObjectId id, int page = 1, int pageSize = 32);
    Task AddToInventory(ObjectId userId, List<ObjectId> items);
    Task RemoveFromInventory(User user, ObjectId itemId, int quantity = 1);
    Task UpdateAuthToken(ObjectId userId, string authToken);
}