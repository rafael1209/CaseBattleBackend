using System.Net.WebSockets;
using CaseBattleBackend.Interfaces;

namespace CaseBattleBackend.Services;

public class ConnectionFactory : IConnectionFactory
{
    public IConnection CreateConnection(WebSocket webSocket)
    {
        return new WebSocketConnection(webSocket);
    }
}