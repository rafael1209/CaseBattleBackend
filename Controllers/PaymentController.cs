using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Middlewares;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CaseBattleBackend.Controllers;

[Route("api/v1/payments")]
public class PaymentController(IConfiguration configuration, ISpPaymentService paymentService) : Controller
{
    private readonly string _spRedirectUrl = configuration["SPWorlds:RedirectUrl"] ??
                                             throw new Exception("SPWorlds:RedirectUrl not configuration");

    private readonly string _spWebhookUrl = configuration["SPWorlds:WebhookUrl"] ??
                                            throw new Exception("SPWorlds:WebhookUrl not configuration");

    [HttpPost]
    [AuthMiddleware]
    public async Task<IActionResult> Donate(DepositRequest deposit)
    {
        // this is a test shit, you need to fix this shit.

        var user = HttpContext.Items["@me"] as User
                   ?? throw new SecurityTokenEncryptionKeyNotFoundException();

        var response = await paymentService.CreatePayment(new TransactionRequest
        {
            Items =
            [
                new Item
                {
                    Name = "Поменять это позже",
                    Count = 1,
                    Price = deposit.Amount,
                    Comment = "Пополнение баланса"
                }
            ],
            RedirectUrl = _spRedirectUrl,
            WebhookUrl = _spWebhookUrl,
            Data = $"{user.Id}"
        });

        return Ok(response);
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromBody] PaymentNotification notification)
    {
        if (!HttpContext.Request.Headers.TryGetValue("X-Body-Hash", out var base64HashValues))
            return Unauthorized(new { message = "Missing or empty X-Body-Hash header." });

        var base64Hash = base64HashValues.FirstOrDefault() ?? throw new Exception("Error X-Body-Hash header");

        await paymentService.HandlePaymentNotification(notification, base64Hash);

        return Ok(new { status = "success" });
    }
}