using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;

namespace CaseBattleBackend.Interfaces;

public interface IItemService
{
    Task<CaseItem> Create(CreateItemRequest request);
}