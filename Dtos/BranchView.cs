using System.Text.Json.Serialization;
using CaseBattleBackend.Models;

namespace CaseBattleBackend.Dtos;

public class BranchView
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("coordinates")]
    public Coordinate? Coordinates { get; set; }

    [JsonPropertyName("imageUrls")]
    public List<Uri>? ImageUrls { get; set; }
}