using CaseBattleBackend.Enums;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Services;

public class TransactionService(ITransactionRepository transactionRepository) : ITransactionService
{
    public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
    {
        return await transactionRepository.CreateTransactionAsync(transaction);
    }

    public async Task<Transaction?> GetTransactionByIdAsync(ObjectId id)
    {
        return await transactionRepository.GetTransactionByIdAsync(id);
    }

    public async Task<List<Transaction>> GetTransactionsByUserIdAsync(ObjectId userId, int page = 1, int pageSize = 20)
    {
        return await transactionRepository.GetTransactionsByUserIdAsync(userId, page, pageSize);
    }

    public async Task<bool> UpdateTransactionStatusAsync(ObjectId transactionId, TransactionStatus status)
    {
        return await transactionRepository.UpdateTransactionStatusAsync(transactionId, status);
    }
}