using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CaseBattleBackend.Controllers;

[Route("api/v1/items")]
public class ItemsController(IItemService itemService) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateItemRequest request)
    {
        var item = await itemService.Create(request);

        return Ok(new { item });
    }
}