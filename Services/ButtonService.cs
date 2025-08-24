using CaseBattleBackend.Enums;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using Discord;
using Discord.Net;

namespace CaseBattleBackend.Services;

public class ButtonService(IUserService userService, ICellService cellService, IItemService itemService, IOrderService orderService) : IButtonService
{
    public async Task<Order> AcceptOrder(string id, ulong currierId)
    {
        var currier = await userService.GetByDiscordId(currierId) ??
                                  throw new Exception($"Courier with Discord ID {currierId} not found.");

        var order = await orderService.GetOrderByIdAsync(id) ??
            throw new Exception($"Order with ID {id} not found.");

        if (order.CourierId != null)
            throw new Exception("");

        await orderService.AddCourier(id, currier.Id);
        await orderService.UpdateStatus(order.Id, OrderStatus.InDelivery);

        await userService.UpdateBalance(currier.Id, order.Price);

        return order;
    }

    public async Task<Order> CompleteOrder(string id, ulong userId)
    {
        var currier = await userService.GetByDiscordId(userId) ??
                      throw new Exception($"Courier with Discord ID {userId} not found.");

        var order = await orderService.GetOrderByIdAsync(id) ??
                    throw new Exception($"Order with ID {id} not found.");

        if (order.CourierId != currier.Id)
            throw new Exception($"Order with ID {id} is not assigned to courier with ID {currier.Id}.");

        var item = await itemService.GetById(order.Item.Id.ToString()) ??
                   throw new Exception($"Item with ID {order.Item.Id} not found.");

        var neededCells = (int)Math.Ceiling((double)item.Amount * order.Item.Amount / (double)item.StackAmount!);
        var cell = await cellService.GetEmptyCell(neededCells);

        await orderService.UpdateCell(order.Id, cell.Id);
        await orderService.UpdateStatus(order.Id, OrderStatus.Delivered);

        return order;
    }

    public async Task<Order> CancelOrder(string id, ulong userId)
    {
        var currier = await userService.GetByDiscordId(userId) ??
                      throw new Exception($"Courier with Discord ID {userId} not found.");

        var order = await orderService.GetOrderByIdAsync(id) ??
                    throw new Exception($"Order with ID {id} not found.");

        if (order.Status == OrderStatus.Accepted)
        {
            if (order.CourierId != currier.Id)
                throw new Exception($"Order with ID {id} is not assigned to courier with ID {currier.Id}.");

            await userService.UpdateBalance(currier.Id, -order.Price);
        }

        await orderService.UpdateStatus(order.Id, OrderStatus.Cancelled);
        await userService.UpdateBalance(order.UserId, order.Price);

        return order;
    }
}