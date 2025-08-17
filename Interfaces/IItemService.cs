using CaseBattleBackend.Dtos;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;

namespace CaseBattleBackend.Interfaces;

public interface IItemService
{
    Task<CaseItem> Create(CreateItemRequest request);
    Task Delete(string id);
    Task<List<CaseItemView>> GetItems(int fromPrice = 0, int page = 1, int pageSize = 20);
    Task<CaseItem?> GetById(string id);
    Task<CaseItemView> GetItemViewById(string id);
    Task<Uri> GetItemImageAsync(CaseItem item);
}