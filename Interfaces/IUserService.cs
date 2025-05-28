using CaseBattleBackend.Dtos;
using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Interfaces;

public interface IUserService
{
    Task<User?> TryGetByMinecraftUuid(string minecraftUuid);
    Task<User> Create(User user);
    Task<User?> GetById(ObjectId id);
    Task<UserInfo> GetUserInfo(User user);
    Task<bool> UpdateBalance(ObjectId id, double amount);
    Task<List<InventoryItemView>> GetInventoryItems(ObjectId userId, int page = 1, int pageSize = 32);
    Task AddToInventory(ObjectId userId, List<CaseItemViewDto> items);
    Task SellItem(User user, string itemId);
}