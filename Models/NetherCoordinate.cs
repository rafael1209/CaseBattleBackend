using System.Text.Json.Serialization;
using CaseBattleBackend.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CaseBattleBackend.Models;

public class NetherCoordinate
{
    [BsonElement("color")]
    [JsonPropertyName("color")]
    [BsonRepresentation(BsonType.String)]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required LineColor Color { get; set; }

    [BsonElement("distance")]
    [JsonPropertyName("distance")]
    public required double Distance { get; set; }
}