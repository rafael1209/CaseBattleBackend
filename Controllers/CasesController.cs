using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Middlewares;
using CaseBattleBackend.Requests;
using Microsoft.AspNetCore.Mvc;

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

            return Ok(new { caseData });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return NotFound();
        }
    }

    [HttpPost("{id}/open")]
    [AuthMiddleware]
    public async Task<IActionResult> OpenCase(string id)
    {
        var winItem = await caseService.OpenCase(id);

        return Ok(new { winItem });
    }

    [HttpGet]
    [AuthMiddleware]
    public async Task<IActionResult> GetCases()
    {
        var cases = await caseService.GetAll();

        return Ok(new { cases });
    }

    [HttpPost]
    [AuthMiddleware]
    public async Task<IActionResult> CreateCase([FromBody] CreateCaseRequest request)
    {
        try
        {
            var newCase = await caseService.Create(request);

            return Ok(new { newCase });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(new { error = e.Message });
        }
    }
}