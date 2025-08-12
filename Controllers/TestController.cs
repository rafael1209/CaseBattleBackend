using CaseBattleBackend.Services;
using Discord;
using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;

namespace CaseBattleBackend.Controllers
{
    [Route("api/[controller]")]
    public class TestController(IDiscordNotificationService notificationService) : Controller
    {
        [HttpGet("ping")]
        public async Task<IActionResult> Test([FromQuery] ulong channelId)
        {
            await notificationService.SendWithdrawRequestAsync("TestUser", 100, "TestCard123");

            return Ok(new { message = "Ping successful!" });
        }
    }
}
