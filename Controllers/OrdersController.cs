using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Middlewares;
using CaseBattleBackend.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
}

public class CreateOrderRequest
{
    public required string ItemId { get; set; }
    [Range(1, 99)]
    public required int Amount { get; set; }
}