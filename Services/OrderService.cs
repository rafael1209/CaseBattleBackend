using CaseBattleBackend.Dtos;
using CaseBattleBackend.Enums;
using CaseBattleBackend.Helpers;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using MongoDB.Bson;

namespace CaseBattleBackend.Services;

public class OrderService(
    IOrderRepository orderRepository,
    IItemService itemService,
    IUserService userService,
    IBranchService branchService,
    ICellService cellService,
    IDiscordNotificationService notificationService) : IOrderService
{
    public async Task<List<Order>> GetAllOrdersAsync()
    {
        var orders = await orderRepository.GetAllOrdersAsync();

        return orders;
    }

    public async Task<Order?> GetOrderByIdAsync(string orderId)
    {
        if (!ObjectId.TryParse(orderId, out var orderObjectId))
            throw new ArgumentException(@"Invalid ObjectId format", nameof(orderId));

        var order = await orderRepository.GetOrderByIdAsync(orderObjectId);

        return order;
    }

    public async Task Confirm(string userId, string orderId)
    {
        if (!ObjectId.TryParse(userId, out var userObjectId))
            throw new ArgumentException(@"Invalid ObjectId format", nameof(userId));

        if (!ObjectId.TryParse(orderId, out var orderObjectId))
            throw new ArgumentException(@"Invalid ObjectId format", nameof(orderId));

        var order = await orderRepository.GetOrderByIdAsync(orderObjectId) ??
                    throw new ArgumentException($@"Order with ID {orderId} not found.", nameof(orderId));

        if (order.UserId != userObjectId)
            throw new UnauthorizedAccessException();

        await cellService.UpdateCellStatus(order.CellId);

        order.Status = OrderStatus.Confirmed;
        await orderRepository.UpdateOrderAsync(order);
    }

    public async Task AddCourier(string orderId, ObjectId courierId)
    {
        if (!ObjectId.TryParse(orderId, out var orderObjectId))
            throw new ArgumentException(@"Invalid ObjectId format", nameof(orderId));
        var order = await orderRepository.GetOrderByIdAsync(orderObjectId) ??
                    throw new ArgumentException($@"Order with ID {orderId} not found.", nameof(orderId));
        order.CourierId = courierId;
        await orderRepository.UpdateOrderAsync(order);
    }

    public async Task<Order> CreateOrderAsync(JwtData jwtData, CreateOrderRequest request)
    {
        if (request.Amount is <= 0 or > 99)
            throw new ArgumentException(@"Amount must be between 1 and 99.", nameof(request.Amount));

        if (!ObjectId.TryParse(jwtData.Id, out var userId))
            throw new ArgumentException(@"Invalid ObjectId format", nameof(jwtData.Id));

        var user = await userService.GetById(userId) ??
                   throw new ArgumentException($@"User with ID {jwtData.Id} not found.", nameof(jwtData.Id));

        if (!ObjectId.TryParse(request.ItemId, out var itemId))
            throw new ArgumentException(@"Invalid ObjectId format", nameof(request.ItemId));

        var branch = await branchService.GetBranchByIdAsync(ObjectId.Parse(request.BranchId)) ??
                     throw new ArgumentException($@"Branch with ID {request.BranchId} not found.", nameof(request.BranchId));

        var inventoryItem = user.Inventory
                .Find(item => item.Id == itemId && item.Amount >= request.Amount) ??
                   throw new ArgumentException($@"Item with ID {request.ItemId} not found in user's inventory.", nameof(request.ItemId));

        var item = await itemService.GetById(request.ItemId) ??
                   throw new ArgumentException($@"Item with ID {request.ItemId} not found in the database.", nameof(request.ItemId));

        if (item.MinecraftId == null)
            throw new ArgumentException(@"Item is not withdrawable.", nameof(request.ItemId));

        await userService.RemoveFromInventory(user, inventoryItem.Id, request.Amount);

        var order = new Order
        {
            UserId = user.Id,
            Item = new InventoryItem(inventoryItem.Id, request.Amount),
            Price = item.Price * request.Amount,
            Status = OrderStatus.Created,
            BranchId = branch.Id
        };

        var (embed, components) = DiscordEmbedBuilder.BuildItemWithdraw(user, new InventoryItemView()
        {
            Item = new CaseItemView()
            {
                Id = item.Id.ToString(),
                Name = item.Name,
                Description = item.Description,
                ImageUrl = await itemService.GetItemImageAsync(item),
                Amount = item.Amount,
                Price = item.Price,
                Rarity = item.Rarity
            },
            Amount = request.Amount
        }, order);

        await notificationService.SendAsync(embed, 1407857443391668355, components); //TODO: Move channel ID to config

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

    public async Task<List<OrderView>> GetOrdersViewByUserId(string userId, int page = 1, int pageSize = 8)
    {
        if (!ObjectId.TryParse(userId, out var userIdObject))
            throw new ArgumentException(@"Invalid ObjectId format", nameof(userId));

        var orders = await orderRepository.GetOrdersByUserIdAsync(userIdObject, page, pageSize);
        var orderViews = new List<OrderView>();
        foreach (var order in orders)
        {
            orderViews.Add(new OrderView
            {
                Id = order.Id.ToString(),
                Branch = await branchService.GetBranchViewById(order.BranchId),
                Item = new InventoryItemView
                {
                    Item = await itemService.GetItemViewById(order.Item.Id.ToString()),
                    Amount = order.Item.Amount
                },
                Status = order.Status,
                Cell = await cellService.GetCellView(order.CellId)
            });
        }

        return orderViews;
    }

    public async Task UpdateStatus(ObjectId orderId, OrderStatus status)
    {
        var order = await orderRepository.GetOrderByIdAsync(orderId) ??
                    throw new ArgumentException($@"Order with ID {orderId} not found.", nameof(orderId));

        order.Status = status;

        await orderRepository.UpdateOrderAsync(order);
    }

    public async Task UpdateCell(ObjectId orderId, ObjectId cellId)
    {
        var order = await orderRepository.GetOrderByIdAsync(orderId) ??
                    throw new ArgumentException($@"Order with ID {orderId} not found.", nameof(orderId));
        order.CellId = cellId;

        await orderRepository.UpdateOrderAsync(order);
    }
}