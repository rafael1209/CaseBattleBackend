using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using CaseBattleBackend.Interfaces;

namespace CaseBattleBackend.Controllers;

[ApiController]
[Route("api/v1/validate")]
public class ValidateController(IAuthorizeService authorizeService) : Controller
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] JsonElement body)
    {
        try
        {
            var authToken = await authorizeService.AuthorizeUser(body);

            return Ok(new { authToken });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            return BadRequest(new { error = e.Message });
        }
    }
}