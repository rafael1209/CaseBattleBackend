using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Middlewares;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CaseBattleBackend.Controllers;

[Route("api/v1/payments")]
public class PaymentController(IUserService userService) : Controller
{
    [HttpPost("deposit")]
    [AuthMiddleware]
    public async Task<IActionResult> Donate([FromBody] DepositRequest deposit)
    {
        var user = HttpContext.Items["@me"] as User
                   ?? throw new SecurityTokenEncryptionKeyNotFoundException();

        var response = await userService.CreatePayment(user, deposit.Amount);

        return Ok(response);
    }

    [HttpPost("withdraw")]
    [AuthMiddleware]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest withdraw)
    {
        var user = HttpContext.Items["@me"] as User
                   ?? throw new SecurityTokenEncryptionKeyNotFoundException();

        await userService.Withdraw(user, withdraw.CardId, withdraw.Amount);

        return Ok();
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