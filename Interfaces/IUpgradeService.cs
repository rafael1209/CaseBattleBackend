using CaseBattleBackend.Dtos;
using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Interfaces;

public interface IUpgradeService
{
    Task<UpdateConfig> GetConfig();
    Task<List<CaseItemView>> GetItems(int minPrice = 0, int page = 1, int pageSize = 16);
    Task<UpgradeResult> UpgradeItem(string userId, List<string> selectedItemIds, string targetItemId);
}