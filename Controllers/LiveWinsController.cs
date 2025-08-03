using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Middlewares;
using Microsoft.AspNetCore.Mvc;

namespace CaseBattleBackend.Controllers;

[Route("api/v1/game-results")]
public class LiveWinsController(IGameResult gameResult) : Controller
{
    [AuthMiddleware]
    [HttpGet]
    public async Task<IActionResult> GetLiveWins()
    {
        var wins = await gameResult.GetLastWins();

        return Ok(new { wins });
    }
}