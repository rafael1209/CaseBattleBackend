using MongoDB.Bson.Serialization.Attributes;

namespace CaseBattleBackend.Models;

public class OverworldCoordinate
{
    [BsonElement("x")]
    public required double X { get; set; }

    [BsonElement("y")]
    public required double Y { get; set; }

    [BsonElement("z")]
    public required double Z { get; set; }
}