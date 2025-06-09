using CaseBattleBackend.Dtos;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using CaseBattleBackend.Responses;
using MongoDB.Bson;

namespace CaseBattleBackend.Interfaces;

public interface IUserService
{
    Task<User?> TryGetByMinecraftUuid(string minecraftUuid);
    Task<User> Create(User user);
    Task<User?> GetById(ObjectId id);
    Task<UserInfo> GetUserInfo(string userId);
    Task<bool> UpdateBalance(ObjectId id, double amount);
    Task<List<InventoryItemView>> GetInventoryItems(string userId, int page = 1, int pageSize = 32);
    Task AddToInventory(ObjectId userId, List<CaseItemViewDto> items);
    Task SellItem(string userId, string itemId, int quantity);
    Task Withdraw(string userId, string cardId, int amount);
    Task<PaymentResponse> CreatePayment(string userId, int amount);
    Task HandlePayment(PaymentNotification notification, string base64Hash);
}