using CaseBattleBackend.Enums;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Middlewares;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace CaseBattleBackend.Controllers;

[Route("api/v1/admin")]
public class AdminController(IUserService userService, ITokenService tokenService) : Controller
{
    [HttpGet]
    //[AuthMiddleware]
    public async Task<IActionResult> Get()
    {
        try
        {
            var user = await userService.Create(new User
            {
                Id = ObjectId.GenerateNewId(),
                DiscordId = 0,
                MinecraftUuid = "uuid",
                Username = "Rafaello",
                Balance = 0,
                AuthToken = tokenService.GenerateToken("rafael1209", PermissionLevel.User),
            });

            return Ok("this is a test message" + new { user });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return NotFound(new { message = e.Message });
        }
    }

    [HttpPost]
    [AuthMiddleware(PermissionLevel.Admin)]
    public async Task<IActionResult> Create(CreateBannerRequest bannerRequest)
    {
        try
        {

            return Ok(new { messgae = "this is a test permission message" });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return NotFound(new { message = e.Message });
        }
    }

    [HttpPost("set-access")]
    public async Task<IActionResult> SetAccess([FromBody] SetAccessRequest request)
    {
        return BadRequest("Under developing");
    }
}