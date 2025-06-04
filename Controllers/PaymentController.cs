using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Middlewares;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using CaseBattleBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net.WebSockets;
using System.Text.Json;
using CaseBattleBackend.Enums;

namespace CaseBattleBackend.Controllers;

[Route("api/v1/payments")]
public class PaymentController(IUserService userService, WebSocketServerService webSocket) : Controller
{
    [HttpPost("deposit")]
    [AuthMiddleware]
    public async Task<IActionResult> Donate([FromBody] DepositRequest deposit)
    {
        var user = HttpContext.Items["@me"] as User
                   ?? throw new SecurityTokenEncryptionKeyNotFoundException();

        try
        {
            var response = await userService.CreatePayment(user, deposit.Amount);

            return Ok(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            return BadRequest(new { message = e.Message });
        }
    }

    [HttpPost("withdraw")]
    [AuthMiddleware]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest withdraw)
    {
        var user = HttpContext.Items["@me"] as User
                   ?? throw new SecurityTokenEncryptionKeyNotFoundException();

        try
        {
            await userService.Withdraw(user, withdraw.CardId, withdraw.Amount);

            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

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

    [HttpPost("test")]
    public async Task<IActionResult> Test()
    {
        var json = JsonSerializer.Serialize(new
        {
            type = "live_win",
            data = new
            {
                UserInfo = new
                {
                    Id = "1234567890",
                    Nickname = "TestUser",
                    AvatarUrl = "https://example.com/avatar.png",
                    Balance = 100.0,
                    Level = 1
                },
            }
        });

        webSocket.PublishToChannel(SubscriptionChannel.LiveWins, json);

        return Ok("Live win sent to subscribers.");
    }

    [HttpPost("test1")]
    public async Task<IActionResult> Test1()
    {
        webSocket.Broadcast(new{message = "Hello World", type = "0"});

        return Ok("Live win sent to subscribers.");
    }
}