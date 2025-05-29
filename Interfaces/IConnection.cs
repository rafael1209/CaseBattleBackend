using System.Net.WebSockets;

namespace CaseBattleBackend.Interfaces;

public interface IConnection
{
    Task<WebSocketCloseStatus?> KeepReceiving();
    Task Send(string message);
    Task Close();
}