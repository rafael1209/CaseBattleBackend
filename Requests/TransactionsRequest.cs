using System.Text.Json.Serialization;

namespace CaseBattleBackend.Requests;

public class TransactionsRequest
{
    [JsonPropertyName("receiver")]
    public string Receiver { get; set; }

    [JsonPropertyName("amount")]
    public int Amount { get; set; }

    [JsonPropertyName("comment")]
    public string Comment { get; set; }
}