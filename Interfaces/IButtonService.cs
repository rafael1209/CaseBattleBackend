using CaseBattleBackend.Dtos;

namespace CaseBattleBackend.Interfaces;

public interface IButtonService
{
    Task<InventoryItemView> GetItemByOrder(string id);
}