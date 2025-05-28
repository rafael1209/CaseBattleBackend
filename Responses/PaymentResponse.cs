using System.Text.Json.Serialization;

namespace CaseBattleBackend.Responses;

public class PaymentResponse
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("card")]
    public string Card { get; set; }
}