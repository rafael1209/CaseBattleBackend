namespace CaseBattleBackend.Interfaces;

public interface IButtonService
{
    Task AcceptOrder(string id, ulong userId);
}