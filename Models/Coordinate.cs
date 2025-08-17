using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace CaseBattleBackend.Models;

public class Coordinate
{
    [BsonElement("overworld")]
    [JsonPropertyName("overworld")]
    public required OverworldCoordinate OverWorld { get; set; }

    [BsonElement("the_nether")]
    [JsonPropertyName("the_nether")]
    public required NetherCoordinate Nether { get; set; }
}