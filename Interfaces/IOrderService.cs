using CaseBattleBackend.Controllers;
using CaseBattleBackend.Enums;
using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Interfaces;

public interface IOrderService
{
    Task<List<Order>> GetAllOrdersAsync();
    Task<Order?> GetOrderByIdAsync(ObjectId orderId);
    Task<Order> CreateOrderAsync(JwtData jwtData, CreateOrderRequest request);
    Task UpdateOrderAsync(Order order);
    Task DeleteOrderAsync(ObjectId orderId);
    Task<List<Order>> GetOrdersByUserIdAsync(ObjectId userId);
    Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status);
}