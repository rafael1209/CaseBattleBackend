using CaseBattleBackend.Interfaces;
using Google.Apis.Auth.OAuth2;

namespace CaseBattleBackend.Services;

public class ButtonService(IUserService userService, IOrderService orderService, IItemService itemService) : IButtonService
{
    public async Task AcceptOrder(string id, ulong currierId)
    {
        var currier = await userService.GetByDiscordId(currierId) ??
                                  throw new Exception($"Courier with Discord ID {currierId} not found.");

        var order = await orderService.GetOrderByIdAsync(id) ??
            throw new Exception($"Order with ID {id} not found.");

        if (order.CourierId != null) return;

        await orderService.AddCourier(id, currier.Id);

        await orderService.UpdateStatus(order.Id, Enums.OrderStatus.Accepted);
    }
}