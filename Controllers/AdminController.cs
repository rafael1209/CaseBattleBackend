using CaseBattleBackend.Enums;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Middlewares;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CaseBattleBackend.Controllers;

[Route("api/v1/admin")]
public class AdminController(IUserService userService, ITokenService tokenService) : Controller
{
    [HttpPost("set-access")]
    [AuthMiddleware(PermissionLevel.Moderator)]
    public async Task<IActionResult> SetAccess([FromBody] SetAccessRequest request)
    {
        try
        {
            var jwtData = HttpContext.Items["@me"] as JwtData
                          ?? throw new UnauthorizedAccessException();

            await userService.SetAccess(jwtData, request);

            return Ok(new { message = "Access level updated successfully." });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            return BadRequest(new { message = e.Message });
        }
    }

    [HttpPost("test")]
    //[AuthMiddleware(PermissionLevel.Owner)]
    public Task<IActionResult> Test([FromBody] string userId)
    {
        try
        {
            var authToken = tokenService.GenerateToken(userId, PermissionLevel.Owner);

            return Task.FromResult<IActionResult>(Ok(new { authToken }));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            return Task.FromResult<IActionResult>(BadRequest(new { message = e.Message }));
        }
    }
}