using CaseBattleBackend.Enums;
using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Interfaces;

public interface IOrderRepository
{
    Task<List<Order>> GetAllOrdersAsync();
    Task<Order?> GetOrderByIdAsync(ObjectId orderId);
    Task<Order> CreateOrderAsync(Order order);
    Task UpdateOrderAsync(Order order);
    Task DeleteOrderAsync(ObjectId orderId);
    Task<List<Order>> GetOrdersByUserIdAsync(ObjectId userId, int page = 1, int pageSize = 8);
    Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status);
}