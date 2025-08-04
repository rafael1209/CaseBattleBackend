using System.Text.Json.Serialization;

namespace CaseBattleBackend.Responses;

public class TransactionErrorResponse
{
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}