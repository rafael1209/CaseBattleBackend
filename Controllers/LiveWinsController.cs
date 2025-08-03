using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Middlewares;
using Microsoft.AspNetCore.Mvc;

namespace CaseBattleBackend.Controllers;

public class LiveWinsController(IGameResult gameResult) : Controller
{
    [AuthMiddleware]
    public async Task<IActionResult> GetLiveWins()
    {
        var wins = await gameResult.GetLastWins();

        return Ok(new { wins });
    }
}