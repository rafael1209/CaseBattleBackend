using CaseBattleBackend.Enums;
using System.Text.Json.Serialization;

namespace CaseBattleBackend.Dtos;

public class OrderView
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("branch")]
    public required BranchView? Branch { get; set; }

    [JsonPropertyName("cell")]
    public required CellView Cell { get; set; }

    [JsonPropertyName("item")]
    public required InventoryItemView Item { get; set; }

    [JsonPropertyName("price")]
    public decimal Price => Item.Item.Price * Item.Amount;

    [JsonPropertyName("status")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderStatus Status { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
}