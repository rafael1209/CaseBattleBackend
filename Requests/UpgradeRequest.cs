using System.Text.Json.Serialization;

namespace CaseBattleBackend.Requests;

public class UpgradeRequest
{
    [JsonPropertyName("selectedItemIds")]
    public required List<string> SelectedItemIds { get; set; }
    [JsonPropertyName("targetItemId")]
    public required string TargetItemId { get; set; }
}