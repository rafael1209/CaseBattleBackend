using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CaseBattleBackend.Models;

public class Branch
{
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

    [BsonElement("name")]
    public required string Name { get; set; }

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("coordinates")]
    public Coordinate? Coordinates { get; set; }

    [BsonElement("imageIds")]
    public List<string> ImageIds { get; set; } = [];

    [BsonElement("cellIds")]
    public List<ObjectId>? Cells { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}