using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CaseBattleBackend.Models;

public class Case
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("name")]
    public required string Name { get; set; }

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("imageId")]
    public string? ImageId { get; set; }

    [BsonElement("items")]
    public required List<ObjectId> Items { get; set; } = [];

    [BsonElement("rtp")]
    public int RtpPercentage { get; set; }

    [BsonElement("price")]
    public required int Price { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}