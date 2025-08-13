using CaseBattleBackend.Controllers;
using CaseBattleBackend.Dtos;
using CaseBattleBackend.Enums;
using CaseBattleBackend.Helpers;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Services;

public class OrderService(IOrderRepository orderRepository, IItemService itemService, IUserService userService, IDiscordNotificationService notificationService, IMinecraftAssets minecraftAssets, IStorageService storageService) : IOrderService
{
    public async Task<List<Order>> GetAllOrdersAsync()
    {
        var orders = await orderRepository.GetAllOrdersAsync();

        return orders;
    }

    public async Task<Order?> GetOrderByIdAsync(ObjectId orderId)
    {
        var order = await orderRepository.GetOrderByIdAsync(orderId);

        return order;
    }

    public async Task<Order> CreateOrderAsync(JwtData jwtData, CreateOrderRequest request)
    {
        if (!ObjectId.TryParse(jwtData.Id, out var userId))
            throw new ArgumentException(@"Invalid ObjectId format", nameof(jwtData.Id));

        var user = await userService.GetById(userId) ??
                   throw new ArgumentException($@"User with ID {jwtData.Id} not found.", nameof(jwtData.Id));

        if (!ObjectId.TryParse(request.ItemId, out var itemId))
            throw new ArgumentException(@"Invalid ObjectId format", nameof(request.ItemId));

        var inventoryItem = user.Inventory
                .Find(item => item.Id == itemId && item.Amount > 0 && item.Amount >= request.Amount) ??
                   throw new ArgumentException($@"Item with ID {request.ItemId} not found in user's inventory.", nameof(request.ItemId));

        var item = await itemService.GetById(request.ItemId) ??
                   throw new ArgumentException($@"Item with ID {request.ItemId} not found in the database.", nameof(request.ItemId));

        if (item.MinecraftId == null)
            throw new ArgumentException(@"Item is not withdrawable.", nameof(request.ItemId));

        await userService.RemoveFromInventory(user, inventoryItem.Id, request.Amount);

        var (embed, components) = DiscordEmbedBuilder.BuildItemWithdraw(user, new CaseItemView
        {
            Id = item.Id.ToString(),
            Name = item.Name,
            Description = item.Description,
            ImageUrl = item.ImageId != null ? await storageService.GetFileUrl(item.ImageId) : item.MinecraftId != null ? await minecraftAssets.GetItemImageAsync(item.MinecraftId) : null,
            Amount = item.Amount,
            Price = item.Price,
            PercentChance = 0,
            Rarity = Rarity.Common,
            IsWithdrawable = false
        }, request.Amount);

        await notificationService.SendAsync(embed, 1256744815715160064, components);

        var order = new Order
        {
            UserId = user.Id,
            ItemId = inventoryItem.Id,
            Amount = inventoryItem.Amount,
            Status = OrderStatus.Created,
        };

        return await orderRepository.CreateOrderAsync(order);
    }

    public async Task UpdateOrderAsync(Order order)
    {
        await orderRepository.UpdateOrderAsync(order);
    }

    public async Task DeleteOrderAsync(ObjectId orderId)
    {
        await orderRepository.DeleteOrderAsync(orderId);
    }

    public async Task<List<Order>> GetOrdersByUserIdAsync(ObjectId userId)
    {
        var orders = await orderRepository.GetOrdersByUserIdAsync(userId);

        return orders;
    }

    public async Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status)
    {
        var orders = await orderRepository.GetOrdersByStatusAsync(status);

        return orders;
    }
}