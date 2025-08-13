using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Middlewares;
using Microsoft.AspNetCore.Mvc;

namespace CaseBattleBackend.Controllers;

[Route("api/v1/orders")]
public class OrdersController(IOrderService orderService) : Controller
{
    [HttpPost]
    [AuthMiddleware]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {

        return Ok();
    }
}

public class CreateOrderRequest
{
    public required string ItemId { get; set; }
    public required int Amount { get; set; }
}