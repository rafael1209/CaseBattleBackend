using CaseBattleBackend.Dtos;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Services;

public class UpgradeService(IUserService userService, IItemService itemService) : IUpgradeService
{
    public async Task<UpdateConfig> GetConfig()
    {
        var config = new UpdateConfig { Rtp = 80 };

        return config;
    }

    public async Task<List<CaseItemView>> GetItems(int minPrice = 0, int page = 1, int pageSize = 20)
    {
        var items = await itemService.GetItems(minPrice, page, pageSize);

        return items;
    }

    public async Task<UpgradeResult> UpgradeItem(string userId, List<string> selectedItemIds, string targetItemId)
    {
        var user = await userService.GetById(ObjectId.Parse(userId))
                   ?? throw new Exception("User not found.");

        var inventoryItemIds = user.Inventory.Select(i => i.Id.ToString()).ToList();

        if (selectedItemIds.Any(id => !inventoryItemIds.Contains(id)))
            throw new Exception("One or more selected items not found in user's inventory.");

        double totalPrice = 0;
        var selectedIds = new List<ObjectId>();
        foreach (var selectedItemId in selectedItemIds)
        {
            var item = await itemService.GetById(selectedItemId)
                       ?? throw new Exception("Item not found.");

            selectedIds.Add(item.Id);

            totalPrice += item.Price;
        }

        var targetItem = await itemService.GetById(targetItemId);

        if (targetItem == null)
            throw new Exception("Target item not found.");

        if (totalPrice >= targetItem.Price)
            throw new Exception("Total price of selected items must be less than the target item's price.");

        foreach (var id in selectedIds)
            await userService.RemoveFromInventory(user, id);

        return new UpgradeResult
        {
            IsSuccess = false,
            Item = null
        };
    }
}