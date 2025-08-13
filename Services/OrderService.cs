using CaseBattleBackend.Controllers;
using CaseBattleBackend.Dtos;
using CaseBattleBackend.Enums;
using CaseBattleBackend.Helpers;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Services;

public class OrderService(IOrderRepository orderRepository, IUserService userService, IDiscordNotificationService notificationService) : IOrderService
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

        var item = user.Inventory.Find(item => item.Id == itemId && item.Amount > request.Amount) ??
                   throw new ArgumentException($@"Item with ID {request.ItemId} not found in user's inventory.", nameof(request.ItemId));

        await userService.RemoveFromInventory(user, item.Id, request.Amount);

        //var (embed, components) = DiscordEmbedBuilder.BuildItemWithdraw("rafael1209", 160, new CaseItemView
        //{
        //    Name = "test tovar",
        //    Rarity = Rarity.Common,
        //    IsWithdrawable = false,
        //    Id = null
        //});

        //await notificationService.SendAsync(user, item, request.Amount);

        var order = new Order
        {
            UserId = user.Id,
            ItemId = item.Id,
            Amount = item.Amount,
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