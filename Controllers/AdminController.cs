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
    public async Task<IActionResult> SetAccess([FromBody] SetAccessRequest request)
    {
        return BadRequest("Under developing");
    }
}