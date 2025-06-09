using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Middlewares;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CaseBattleBackend.Controllers;

[Route("api/v1/users/me")]
public class UsersController(IUserService userService) : Controller
{
    [HttpGet]
    [AuthMiddleware]
    public async Task<IActionResult> GetMe()
    {
        var jwtData = HttpContext.Items["@me"] as JwtData
                   ?? throw new SecurityTokenEncryptionKeyNotFoundException();

        try
        {
            var userInfo = await userService.GetUserInfo(jwtData.Id); // TODO: IMPORTANT: Create a model for jwt user info

            return Ok(userInfo);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpGet("inventory")]
    [AuthMiddleware]
    public async Task<IActionResult> GetInventory([FromQuery] int page = 1, int pageSize = 10)
    {
        var user = HttpContext.Items["@me"] as User
                   ?? throw new SecurityTokenEncryptionKeyNotFoundException();

        try
        {
            var inventoryItems = await userService.GetInventoryItems(user.Id, page, pageSize);

            return Ok(inventoryItems);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpPost("inventory/{itemId}/sell")]
    [AuthMiddleware]
    public async Task<IActionResult> SellItem([FromRoute] string itemId, SellInvItemRequest request)
    {
        var user = HttpContext.Items["@me"] as User
                   ?? throw new SecurityTokenEncryptionKeyNotFoundException();

        try
        {
            await userService.SellItem(user, itemId, request.Quantity);

            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(new { message = e.Message });
        }
    }
}