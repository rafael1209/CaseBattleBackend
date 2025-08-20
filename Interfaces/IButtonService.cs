using CaseBattleBackend.Models;

namespace CaseBattleBackend.Interfaces;

public interface IButtonService
{
    Task<Order> AcceptOrder(string id, ulong userId);
}