using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using CaseBattleBackend.Responses;

namespace CaseBattleBackend.Interfaces;

public interface ISpPaymentService
{
    Task<PaymentResponse> CreatePayment(string data, int amount);
    Task HandlePaymentNotification(PaymentNotification notification, string base64Hash);
    Task<TransactionsResponse> SendTransaction(string cardNumber, int amount);
}