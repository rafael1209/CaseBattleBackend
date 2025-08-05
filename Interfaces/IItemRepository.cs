using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Interfaces;

public interface IItemRepository
{
    Task<CaseItem> Create(CaseItem item);
    Task<CaseItem?> GetById(ObjectId id);
    Task<List<CaseItem>> GetTopByMaxPrice(double maxPrice, int limit);
    Task<List<CaseItem>> Get(int fromPrice = 0, int page = 1, int pageSize = 20);
}