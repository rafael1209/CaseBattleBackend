using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CaseBattleBackend.Controllers;

[Route("api/v1/cases")]
public class CasesController(ICaseService caseService) : Controller
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCase(string id)
    {
        var caseData = await caseService.GetById(id);

        return Ok(new { caseData });
    }

    [HttpPost("{id}/open")]
    public async Task<IActionResult> OpenCase(string id)
    {
        var winItem = await caseService.OpenCase(id);

        return Ok(new { winItem });
    }

    [HttpGet]
    public async Task<IActionResult> GetCases()
    {
        var cases = await caseService.GetAll();

        return Ok(new { cases });
    }

    [HttpPost]
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