using CaseBattleBackend.Enums;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Middlewares;
using CaseBattleBackend.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CaseBattleBackend.Controllers;

[Route("api/v1/branches")]
public class BranchesController(IBranchService branchService) : Controller
{
    [HttpGet]
    [AuthMiddleware]
    public async Task<IActionResult> Get()
    {
        var branches = await branchService.GetAllBranchesAsync();

        return Ok(branches);
    }

    [HttpPost]
    [AuthMiddleware(PermissionLevel.Admin)]
    public async Task<IActionResult> CreateBranch([FromForm] CreateBranchRequest request)
    {
        var branches = await branchService.CreateBranchAsync(request);

        return Ok(branches);
    }
}