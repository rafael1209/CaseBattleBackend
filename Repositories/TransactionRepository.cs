using CaseBattleBackend.Enums;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CaseBattleBackend.Repositories;

public class TransactionRepository(IMongoDbContext context) : ITransactionRepository
{
    private readonly IMongoCollection<Transaction> _transactions = context.TransactionsCollection;

    public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
    {
        if (transaction == null)
            throw new ArgumentNullException(nameof(transaction), "Transaction cannot be null");
        try
        {
            await _transactions.InsertOneAsync(transaction);
            return transaction;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to create transaction", ex);
        }
    }

    public async Task<Transaction?> GetTransactionByIdAsync(ObjectId id)
    {
        var transaction = await _transactions.Find(t => t.Id == id).FirstOrDefaultAsync();
        if (transaction == null)
            throw new Exception("Transaction not found");
        return transaction;
    }

    public async Task<List<Transaction>> GetTransactionsByUserIdAsync(ObjectId userId, int page = 1, int pageSize = 20)
    {
        var filter = Builders<Transaction>.Filter.Eq(t => t.UserId, userId);
        var transactions = await _transactions.Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
        if (transactions.Count == 0)
            throw new Exception("No transactions found for this user");
        return transactions;
    }

    public async Task<bool> UpdateTransactionStatusAsync(ObjectId transactionId, TransactionStatus status)
    {
        var update = Builders<Transaction>.Update.Set(t => t.Status, status);
        var result = await _transactions.UpdateOneAsync(t => t.Id == transactionId, update);
        if (result.ModifiedCount == 0)
            throw new Exception("Failed to update transaction status");
        return true;
    }
}