using System.Text.Json.Serialization;

namespace CaseBattleBackend.Responses;

public class TransactionsResponse
{
    [JsonPropertyName("balance")]
    public int Balance { get; set; }
}