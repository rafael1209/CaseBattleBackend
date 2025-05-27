using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Middlewares;
using CaseBattleBackend.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CaseBattleBackend.Controllers;

[Route("api/v1/items")]
public class ItemsController(IItemService itemService) : Controller
{
    [HttpGet]
    [AuthMiddleware]
    public async Task<IActionResult> GetItems()
    {
        var items = await itemService.GetItems();

        return Ok(new { items });
    }

    [HttpPost]
    [AuthMiddleware]
    public async Task<IActionResult> Create([FromBody] CreateItemRequest request)
    {
        var item = await itemService.Create(request);

        return Ok(new { item });
    }
}