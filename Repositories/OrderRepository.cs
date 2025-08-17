using CaseBattleBackend.Enums;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
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

    public async Task UpdateOrderAsync(Order order)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.Id, order.Id);

        await _orders.ReplaceOneAsync(filter, order);
    }

    public Task DeleteOrderAsync(ObjectId orderId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Order>> GetOrdersByUserIdAsync(ObjectId userId, int page = 1, int pageSize = 8)
    {
        if (page < 1) page = 1;
        if (pageSize is < 1 or > 16) pageSize = 16;

        return await _orders
            .Find(o => o.UserId == userId)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }


    public Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status)
    {
        throw new NotImplementedException();
    }
}