using System.Text.Json.Serialization;
using CaseBattleBackend.Dtos;

namespace CaseBattleBackend.Models;

public class UpgradeResult
{
    [JsonPropertyName("success")]
    public bool IsSuccess { get; set; }
    [JsonPropertyName("item")]
    public CaseItemView? Item { get; set; }
}