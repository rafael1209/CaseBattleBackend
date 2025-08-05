using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Middlewares;
using Microsoft.AspNetCore.Mvc;

namespace CaseBattleBackend.Controllers;

[Route("api/v1/game-results")]
public class LiveWinsController(IGameResult gameResult) : Controller
{
    [HttpGet]
    [AuthMiddleware]
    [RateLimit(10)]
    public async Task<IActionResult> GetLiveWins()
    {
        try
        {
            var wins = await gameResult.GetLastWins();

            return Ok(new { wins });
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }
}