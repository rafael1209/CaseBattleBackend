using CaseBattleBackend.Enums;
using System.Text.Json.Serialization;

namespace CaseBattleBackend.Dtos;

public class CaseItemView
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Uri? ImageUrl { get; set; }
    public int Amount { get; set; }
    public double Price { get; set; }
    public double? PercentChance { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required Rarity Rarity { get; set; }
}