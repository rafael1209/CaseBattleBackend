using CaseBattleBackend.Helpers;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Requests;
using CaseBattleBackend.Responses;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using MongoDB.Bson;

namespace CaseBattleBackend.Services;

public class SpPaymentService(IConfiguration configuration) : ISpPaymentService
{
    private readonly string _spCardToken = configuration["SPWorlds:CardToken"] ??
                                           throw new Exception("SPWorlds:CardToken not configuration");
    private readonly string _spCardId = configuration["SPWorlds:CardId"] ??
                                           throw new Exception("SPWorlds:CardId not configuration");
    private readonly string _spRedirectUrl = configuration["SPWorlds:RedirectUrl"] ??
                                             throw new Exception("SPWorlds:RedirectUrl not configuration");
    private readonly string _spWebhookUrl = configuration["SPWorlds:WebhookUrl"] ??
                                            throw new Exception("SPWorlds:WebhookUrl not configuration");

    private const string SpBaseUrl = "https://spworlds.ru/api/";

    public async Task<PaymentResponse> CreatePayment(string data, int amount)
    {
        const int maxPricePerItem = 1728;

        var token = SpValidate.GenerateToken(_spCardId, _spCardToken);

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var items = new List<Item>();
        while (amount > maxPricePerItem)
        {
            items.Add(new Item
            {
                Count = 1,
                Price = maxPricePerItem,
            });
            amount -= maxPricePerItem;
        }

        if (amount > 0)
        {
            items.Add(new Item
            {
                Count = 1,
                Price = amount,
            });
        }

        var transaction = new TransactionRequest
        {
            Items = items,
            RedirectUrl = _spRedirectUrl,
            WebhookUrl = _spWebhookUrl,
            Data = data
        };

        var json = JsonSerializer.Serialize(transaction);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync($"{SpBaseUrl}public/payments", content);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error creating payment: {response.StatusCode}");

        var responseContent = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<PaymentResponse>(responseContent)
               ?? throw new Exception($"Error creating payment: {response.StatusCode}");
    }

    public Task HandlePaymentNotification(PaymentNotification notification, string base64Hash)
    {
        var bodyString = JsonSerializer.Serialize(notification);

        if (!SpValidate.ValidateWebhook(bodyString, base64Hash, _spCardToken))
            throw new Exception();

        return Task.CompletedTask;
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
            Comment = "Вывод средств DiamondDrop"
        };

        var json = JsonSerializer.Serialize(transaction);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync($"{SpBaseUrl}public/transactions", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
            return JsonSerializer.Deserialize<TransactionsResponse>(responseContent) ??
                   throw new Exception($"Error creating payment: {response.StatusCode}");

        try
        {
            var errorResponse = JsonSerializer.Deserialize<TransactionErrorResponse>(responseContent);

            if (errorResponse?.Error == "error.public.transactions.receiverCardNotFound")
                throw new Exception("Receiver card not found.");

            throw new Exception($"Payment failed: {errorResponse?.Error ?? response.StatusCode.ToString()}");
        }
        catch (JsonException)
        {
            throw new Exception($"Error creating payment: {response.StatusCode} - {responseContent}");
        }
    }
}