using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CaseBattleBackend.Models;

public class Banner
{
    [BsonId]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

    [BsonElement("imageId")]
    public string ImageId { get; set; }

    [BsonElement("url")]
    public Uri Url { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}