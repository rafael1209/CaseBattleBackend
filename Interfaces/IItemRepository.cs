using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Interfaces;

public interface IItemRepository
{
    Task<CaseItem> Create(CaseItem item);
    Task<CaseItem> GetById(ObjectId id);
}