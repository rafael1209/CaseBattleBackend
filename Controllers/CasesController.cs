using CaseBattleBackend.Enums;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Middlewares;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CaseBattleBackend.Controllers;

[Route("api/v1/cases")]
public class CasesController(ICaseService caseService) : Controller
{
    [HttpGet("{id}")]
    [AuthMiddleware]
    public async Task<IActionResult> GetCase(string id)
    {
        try
        {
            var caseData = await caseService.GetById(id);

            return Ok(caseData);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return NotFound(new { message = e.Message });
        }
    }

    [HttpPost("{caseId}/opens")]
    [AuthMiddleware]
    public async Task<IActionResult> OpenCase(string caseId, [FromQuery] int amount = 1, [FromQuery] bool demo = false)
    {
        try
        {
            var user = HttpContext.Items["@me"] as User
                       ?? throw new SecurityTokenEncryptionKeyNotFoundException();

            var winItems = await caseService.OpenCase(user, caseId, amount, demo);

            return Ok(winItems);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpGet]
    [AuthMiddleware]
    public async Task<IActionResult> GetCases([FromQuery] int page = 1, int pageSize = 15)
    {
        try
        {
            var cases = await caseService.GetAll(page, pageSize);

            return Ok(new { cases });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpPost]
    [AuthMiddleware(PermissionLevel.Admin)]
    public async Task<IActionResult> CreateCase([FromForm] CreateCaseRequest request)
    {
        try
        {
            var newCase = await caseService.Create(request);

            return Ok(new { newCase });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(new { message = e.Message });
        }
    }
}