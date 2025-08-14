using CaseBattleBackend.Dtos;
using CaseBattleBackend.Enums;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Services;

public class ButtonService(IOrderService orderService, IItemService itemService) : IButtonService
{
    public async Task<InventoryItemView> GetItemByOrder(string id)
    {
        if (!ObjectId.TryParse(id, out var orderId))
            throw new ArgumentException(@"Invalid ObjectId format", nameof(id));

        var order = await orderService.GetOrderByIdAsync(orderId) ??
                    throw new ArgumentException($@"Order with ID {id} not found.", nameof(id));

        var item = await itemService.GetById(order.Item.Id.ToString())
            ?? throw new ArgumentException($@"Item with ID {order.Item.Id} not found in the database.", nameof(order.Item.Id));

        return new InventoryItemView
        {
            Item = new CaseItemView
            {
                Id = item.Id.ToString(),
                Name = item.Name,
                Description = item.Description,
                Amount = item.Amount,
                Price = item.Price,
                Rarity = item.Rarity,
                IsWithdrawable = true,
            },
            Amount = order.Item.Amount
        };
    }
}