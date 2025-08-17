using CaseBattleBackend.Enums;
using System.Text.Json.Serialization;

namespace CaseBattleBackend.Dtos;

public class CaseItemView
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Uri? ImageUrl { get; set; }
    public int Amount { get; set; }
    public decimal Price { get; set; }
    public double? PercentChance { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Rarity Rarity { get; set; }
    public bool IsWithdrawable { get; set; }
}