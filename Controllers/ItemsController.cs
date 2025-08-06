using CaseBattleBackend.Enums;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Middlewares;
using CaseBattleBackend.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CaseBattleBackend.Controllers;

[Route("api/v1/items")]
[AuthMiddleware(PermissionLevel.Admin)]
public class ItemsController(IItemService itemService) : Controller
{
    [HttpGet]
    [AuthMiddleware(PermissionLevel.Admin)]
    public async Task<IActionResult> GetItems([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var items = await itemService.GetItems(0, page, pageSize);

            return Ok(new { items });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpPost]
    [AuthMiddleware(PermissionLevel.Admin)]
    public async Task<IActionResult> Create([FromForm] CreateItemRequest request)
    {
        try
        {
            var item = await itemService.Create(request);

            return Ok(new { item });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }
}