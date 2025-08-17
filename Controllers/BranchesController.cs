using CaseBattleBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CaseBattleBackend.Controllers;

[Route("api/v1/branches")]
public class BranchesController(IBranchService branchService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return View();
    }
}