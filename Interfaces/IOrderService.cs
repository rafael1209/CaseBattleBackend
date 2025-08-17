using CaseBattleBackend.Dtos;
using CaseBattleBackend.Enums;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
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
    Task<List<OrderView>> GetOrdersViewByUserId(string userId, int page = 1, int pageSize = 8);
}