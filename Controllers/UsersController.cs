using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Middlewares;
using CaseBattleBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CaseBattleBackend.Controllers;

[Route("api/v1/users/@me")]
public class UsersController(IUserService userService) : Controller
{
    [HttpGet]
    [AuthMiddleware]
    public async Task<IActionResult> GetMe()
    {
        var user = HttpContext.Items["@me"] as User
                   ?? throw new SecurityTokenEncryptionKeyNotFoundException();

        var userInfo = await userService.GetUserInfo(user);

        return Ok(userInfo);
    }

    [HttpGet("inventory")]
    [AuthMiddleware]
    public async Task<IActionResult> GetInventory([FromQuery] int page = 1, int pageSize = 10)
    {
        var user = HttpContext.Items["@me"] as User
                   ?? throw new SecurityTokenEncryptionKeyNotFoundException();

        var inventoryItems = await userService.GetInventoryItems(user.Id, page, pageSize);

        return Ok(inventoryItems);
    }
}