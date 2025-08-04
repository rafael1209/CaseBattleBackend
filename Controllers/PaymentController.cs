using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Middlewares;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CaseBattleBackend.Controllers;

[Route("api/v1/payments")]
public class PaymentController(IUserService userService) : Controller
{
    [HttpPost("deposit")]
    [AuthMiddleware]
    [RateLimit(5)]
    public async Task<IActionResult> Donate([FromBody] DepositRequest deposit)
    {
        var jwtData = HttpContext.Items["@me"] as JwtData
                      ?? throw new UnauthorizedAccessException();

        try
        {
            var response = await userService.CreatePayment(jwtData.Id, deposit.Amount);

            return Ok(response);
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [HttpPost("withdraw")]
    [AuthMiddleware]
    [RateLimit(5)]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest withdraw)
    {
        var jwtData = HttpContext.Items["@me"] as JwtData
                      ?? throw new UnauthorizedAccessException();

        try
        {
            await userService.Withdraw(jwtData.Id, withdraw.CardId, withdraw.Amount);

            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromBody] PaymentNotification notification)
    {
        if (!HttpContext.Request.Headers.TryGetValue("X-Body-Hash", out var base64HashValues))
            return Unauthorized(new { message = "Missing or empty X-Body-Hash header." });

        var base64Hash = base64HashValues.FirstOrDefault() ?? throw new Exception("Error X-Body-Hash header");

        await userService.HandlePayment(notification, base64Hash);

        return Ok(new { status = "success" });
    }
}