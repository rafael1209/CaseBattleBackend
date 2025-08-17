using System.Text.Json.Serialization;
using CaseBattleBackend.Models;
using MongoDB.Bson.Serialization.Attributes;

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
    public required List<Coordinate> Coordinates { get; set; }

    [JsonPropertyName("imageUrls")]
    public List<string>? ImageUrls { get; set; }

    [BsonElement("cell")]
    public required CellView Cell { get; set; }
}