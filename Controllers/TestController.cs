using CaseBattleBackend.Dtos;
using CaseBattleBackend.Enums;
using CaseBattleBackend.Helpers;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace CaseBattleBackend.Controllers;

[Route("api/[controller]")]
public class TestController(IDiscordNotificationService notificationService) : Controller
{
    [HttpGet("ping")]
    public async Task<IActionResult> Test([FromQuery] ulong channelId)
    {
        return Ok(new { message = "Ping successful!" });
    }
}