using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Middlewares;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CaseBattleBackend.Controllers;

[Route("api/v1/users/me")]
public class UsersController(IUserService userService) : Controller
{
    [HttpGet]
    [AuthMiddleware]
    public async Task<IActionResult> GetMe()
    {
        var jwtData = HttpContext.Items["@me"] as JwtData
                   ?? throw new UnauthorizedAccessException();

        try
        {
            var userInfo = await userService.GetUserInfo(jwtData.Id);

            return Ok(userInfo);
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [HttpGet("inventory")]
    [AuthMiddleware]
    public async Task<IActionResult> GetInventory([FromQuery] int page = 1, int pageSize = 10)
    {
        var jwtData = HttpContext.Items["@me"] as JwtData
                      ?? throw new UnauthorizedAccessException();

        try
        {
            var inventoryItems = await userService.GetInventoryItems(jwtData.Id, page, pageSize);

            return Ok(inventoryItems);
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [HttpPost("inventory/{itemId}/sell")]
    [AuthMiddleware]
    public async Task<IActionResult> SellItem([FromRoute] string itemId, SellInvItemRequest request)
    {
        var jwtData = HttpContext.Items["@me"] as JwtData
                      ?? throw new UnauthorizedAccessException();

        try
        {
            await userService.SellItem(jwtData.Id, itemId, request.Quantity);

            return Ok();
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }
}