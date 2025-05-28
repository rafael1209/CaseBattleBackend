using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CaseBattleBackend.Requests;

public class TransactionRequest
{
    [JsonPropertyName("items")]
    public List<Item> Items { get; set; }

    [JsonPropertyName("redirectUrl")]
    public string RedirectUrl { get; set; }

    [JsonPropertyName("webhookUrl")]
    public string WebhookUrl { get; set; }

    [JsonPropertyName("data")]
    [MaxLength(100)]
    public string? Data { get; set; }
}