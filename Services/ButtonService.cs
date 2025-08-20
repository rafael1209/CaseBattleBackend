using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;

namespace CaseBattleBackend.Services;

public class ButtonService(IUserService userService, IOrderService orderService, IItemService itemService) : IButtonService
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

        await orderService.UpdateStatus(order.Id, Enums.OrderStatus.Accepted);

        return order;
    }

    public async Task<Order> CompleteOrder(string id, ulong userId)
    {
        var currier = await userService.GetByDiscordId(userId) ??
                      throw new Exception($"Courier with Discord ID {userId} not found.");

        var order = await orderService.GetOrderByIdAsync(id) ??
                    throw new Exception($"Order with ID {id} not found.");

        await orderService.UpdateStatus(order.Id, Enums.OrderStatus.Confirmed);

        return order;
    }
}