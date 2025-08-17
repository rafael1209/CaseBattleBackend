using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Middlewares;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CaseBattleBackend.Controllers;

[Route("api/v1/orders")]
public class OrdersController(IOrderService orderService) : Controller
{
    [HttpPost]
    [AuthMiddleware]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var jwtData = HttpContext.Items["@me"] as JwtData
                      ?? throw new UnauthorizedAccessException();

        await orderService.CreateOrderAsync(jwtData, request);

        return Ok();
    }

    [HttpGet]
    [AuthMiddleware]
    public async Task<IActionResult> GetOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 15)
    {
        var jwtData = HttpContext.Items["@me"] as JwtData
                      ?? throw new UnauthorizedAccessException();

        var orders = await orderService.GetOrdersViewByUserId(jwtData.Id, page, pageSize);

        return Ok(orders);
    }
}