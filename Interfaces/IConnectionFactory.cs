using System.Net.WebSockets;

namespace CaseBattleBackend.Interfaces;

public interface IConnectionFactory
{
    IConnection CreateConnection(WebSocket webSocket);
}