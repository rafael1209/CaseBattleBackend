using CaseBattleBackend.Enums;
using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction> CreateTransactionAsync(Transaction transaction);
    Task<Transaction?> GetTransactionByIdAsync(ObjectId id);
    Task<List<Transaction>> GetTransactionsByUserIdAsync(ObjectId userId, int page = 1, int pageSize = 20);
    Task<bool> UpdateTransactionStatusAsync(ObjectId transactionId, TransactionStatus status);
}