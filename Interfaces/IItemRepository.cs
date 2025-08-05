using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Interfaces;

public interface IItemRepository
{
    Task<CaseItem> Create(CaseItem item);
    Task<CaseItem?> GetById(ObjectId id);
    Task<List<CaseItem>> GetTopByMaxPrice(double maxPrice, int limit);
    Task<List<CaseItem>> Get();
    Task AddToInventory(ObjectId userId, List<InventoryItem> items);
}