using CaseBattleBackend.Interfaces;
using System.Net.WebSockets;
using System.Text;

namespace CaseBattleBackend.Services;

public class WebSocketConnection(WebSocket webSocket) : IConnection
{
    public async Task<WebSocketCloseStatus?> KeepReceiving()
    {
        WebSocketReceiveResult message;
        do
        {
            using var memoryStream = new MemoryStream();
            message = await ReceiveMessage(memoryStream);
            if (message.Count > 0)
            {
                var receivedMessage = Encoding.UTF8.GetString(memoryStream.ToArray());
                Console.WriteLine($"Received message {receivedMessage}");
                await Send(receivedMessage);
            }
        } while (message.MessageType != WebSocketMessageType.Close);

        return message.CloseStatus;
    }

    private async Task<WebSocketReceiveResult> ReceiveMessage(Stream memoryStream)
    {
        var readBuffer = new ArraySegment<byte>(new byte[4 * 1024]);
        WebSocketReceiveResult result;
        do
        {
            result = await webSocket.ReceiveAsync(readBuffer, CancellationToken.None);
            await memoryStream.WriteAsync(readBuffer.Array, readBuffer.Offset, result.Count,
                CancellationToken.None);
        } while (!result.EndOfMessage);

        return result;
    }

    public async Task Send(string message)
    {
        var bytes = Encoding.UTF8.GetBytes(message);
        await webSocket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true,
            CancellationToken.None);
    }

    public async Task Close()
    {
        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
    }
}