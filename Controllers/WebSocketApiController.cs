using System.Net;
using CaseBattleBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CaseBattleBackend.Controllers
{
    [Route("/api/ws")]
    [ApiController]
    public class WebSocketApiController(
        IConnectionFactory connectionFactory,
        IConnectionManager connectionManager
        /*, IConnection connection*/)
        : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var context = ControllerContext.HttpContext;

            if (!context.WebSockets.IsWebSocketRequest) 
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);

            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            Console.WriteLine($"Accepted connection '{context.Connection.Id}'");
            var connection = connectionFactory.CreateConnection(webSocket);
            await connectionManager.HandleConnection(connection);

            return new EmptyResult();

        }

        //[HttpPost]
        //public async Task<IActionResult> Post()
        //{
        //    await connection.Send("Hello from WebSocket API!");

        //    return Ok(new { message = "Message sent to WebSocket connection." });
        //}
    }
}