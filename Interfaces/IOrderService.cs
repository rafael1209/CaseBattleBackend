using CaseBattleBackend.Dtos;
using CaseBattleBackend.Enums;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using MongoDB.Bson;

namespace CaseBattleBackend.Interfaces;

public interface IOrderService
{
    Task<Order?> GetOrderByIdAsync(string orderId);
    Task AddCourier(string orderId, ObjectId courierId);
    Task<Order> CreateOrderAsync(JwtData jwtData, CreateOrderRequest request);
    Task<List<Order>> GetOrdersByUserIdAsync(ObjectId userId);
    Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status);
    Task<List<OrderView>> GetOrdersViewByUserId(string userId, int page = 1, int pageSize = 8);
}