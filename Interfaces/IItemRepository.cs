using CaseBattleBackend.Models;

namespace CaseBattleBackend.Interfaces;

public interface IItemRepository
{
    Task<CaseItem> Create(CaseItem item);
}