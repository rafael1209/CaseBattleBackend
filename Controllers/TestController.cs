using Microsoft.AspNetCore.Mvc;

namespace CaseBattleBackend.Controllers;

[Route("api/[controller]")]
public class TestController() : Controller
{
    [HttpGet("ping")]
    public async Task<IActionResult> Test([FromQuery] ulong channelId)
    {
        return Ok(new { message = "Ping successful!" });
    }
}