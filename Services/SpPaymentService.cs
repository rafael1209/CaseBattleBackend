using CaseBattleBackend.Helpers;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Requests;
using CaseBattleBackend.Responses;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using MongoDB.Bson;

namespace CaseBattleBackend.Services;

public class SpPaymentService(IConfiguration configuration, IUserService userService) : ISpPaymentService
{
    private readonly string _spCardToken = configuration["SPWorlds:CardToken"] ??
                                           throw new Exception("SPWorlds:CardToken not configuration");

    private readonly string _spCardId = configuration["SPWorlds:CardId"] ??
                                           throw new Exception("SPWorlds:CardId not configuration");

    private const string SpBaseUrl = "https://spworlds.ru/api/";

    public async Task<PaymentResponse> CreatePayment(TransactionRequest request)
    {
        var token = SpValidate.GenerateToken(_spCardId, _spCardToken);

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync($"{SpBaseUrl}public/payments", content);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error creating payment: {response.StatusCode}");

        var responseContent = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<PaymentResponse>(responseContent) ??
               throw new Exception($"Error creating payment: {response.StatusCode}");
    }

    public async Task HandlePaymentNotification(PaymentNotification notification, string base64Hash)
    {
        var bodyString = JsonSerializer.Serialize(notification);

        if (!SpValidate.ValidateWebhook(bodyString, base64Hash, _spCardToken))
            throw new Exception();

        if (!ObjectId.TryParse(notification.Data, out var id))
            throw new Exception("Error Handle Payment notification.Data parse to ObjectId");

        await userService.UpdateBalance(id, (int)notification.Amount);
    }

    public async Task<TransactionsResponse> SendTransaction(string cardNumber, int amount)
    {
        var token = SpValidate.GenerateToken(_spCardId, _spCardToken);

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var transaction = new TransactionsRequest
        {
            Receiver = cardNumber,
            Amount = amount,
            Comment = "Change this"
        };

        var json = JsonSerializer.Serialize(transaction);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync($"{SpBaseUrl}public/transactions", content);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error creating payment: {response.StatusCode}");

        var responseContent = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<TransactionsResponse>(responseContent) ??
               throw new Exception($"Error creating payment: {response.StatusCode}");
    }
}