namespace CaseBattleBackend.Interfaces;

public interface IConnectionManager
{
    Task HandleConnection(IConnection connection);
}