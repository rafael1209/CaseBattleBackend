using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Middlewares;
using CaseBattleBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CaseBattleBackend.Controllers;

[Route("api/v1/users")]
public class UsersController(IUserService userService) : Controller
{
    [HttpGet("@me")]
    [AuthMiddleware]
    public async Task<IActionResult> GetMe()
    {
        var user = HttpContext.Items["@me"] as User
                   ?? throw new SecurityTokenEncryptionKeyNotFoundException();

        var userInfo = await userService.GetUserInfo(user);

        return Ok(userInfo);
    }
}