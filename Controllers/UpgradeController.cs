using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Middlewares;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CaseBattleBackend.Controllers;

[Route("api/v1/upgrade")]
public class UpgradeController(IUpgradeService upgradeService) : Controller
{
    [HttpGet]
    [AuthMiddleware]
    public async Task<IActionResult> Upgrade()
    {
        var conf = await upgradeService.GetConfig();

        return Ok(conf);
    }

    [HttpGet("items")]
    [AuthMiddleware]
    public async Task<IActionResult> GetUpgradeItems([FromQuery] int minPrice = 0, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var upgradeItems = await upgradeService.GetItems(minPrice, page, pageSize);

        return Ok(upgradeItems);
    }

    [HttpPost]
    [AuthMiddleware]
    public async Task<IActionResult> UpgradeItems(UpgradeRequest request)
    {
        var jwtData = HttpContext.Items["@me"] as JwtData
                      ?? throw new UnauthorizedAccessException();

        try
        {
            var upgradeResult =
                await upgradeService.UpgradeItem(jwtData.Id, request.SelectedItemIds, request.TargetItemId);

            return Ok(upgradeResult);
        }
        catch (Exception e)
        {
            return BadRequest(new { e.Message });
        }
    }
}