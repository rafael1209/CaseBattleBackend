using System.Text.Json.Serialization;

namespace CaseBattleBackend.Dtos;

public class CellView
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }
}