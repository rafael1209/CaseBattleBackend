using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CaseBattleBackend.Requests;

public class Item
{
    [JsonPropertyName("name")]
    [StringLength(32, MinimumLength = 3)]
    public required string Name { get; set; }

    [JsonPropertyName("count")]
    [Range(1, 9999)]
    public int Count { get; set; }

    [JsonPropertyName("price")]
    [Range(1, 1728)]
    public int Price { get; set; }

    [JsonPropertyName("comment")]
    [StringLength(64, MinimumLength = 3)]
    public string? Comment { get; set; }
}