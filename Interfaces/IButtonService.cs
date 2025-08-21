using CaseBattleBackend.Models;

namespace CaseBattleBackend.Interfaces;

public interface IButtonService
{
    Task<Order> AcceptOrder(string id, ulong userId);
    Task<Order> CompleteOrder(string id, ulong userId);
    Task<Order> CancelOrder(string id, ulong userId);
}