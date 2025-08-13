using CaseBattleBackend.Enums;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CaseBattleBackend.Repositories;

public class OrderRepository(IMongoDbContext context) : IOrderRepository
{
    private readonly IMongoCollection<Order> _orders = context.OrdersCollection;

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        var ordersCursor = await _orders.FindAsync(_ => true);

        return await ordersCursor.ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(ObjectId orderId)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.Id, orderId);
        var order = await _orders.Find(filter).FirstOrDefaultAsync();

        return order;
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        await _orders.InsertOneAsync(order);

        return order;
    }

    public Task UpdateOrderAsync(Order order)
    {
        throw new NotImplementedException();
    }

    public Task DeleteOrderAsync(ObjectId orderId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Order>> GetOrdersByUserIdAsync(ObjectId userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status)
    {
        throw new NotImplementedException();
    }
}