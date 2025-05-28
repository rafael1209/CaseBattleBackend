using System.Text.Json.Serialization;

namespace CaseBattleBackend.Requests;

public class PaymentNotification
{
    [JsonPropertyName("data")]
    public string Data { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("payer")]
    public string Payer { get; set; }
}