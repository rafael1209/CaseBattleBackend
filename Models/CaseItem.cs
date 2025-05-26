using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CaseBattleBackend.Models;

public class CaseItem
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("name")]
    public required string Name { get; set; }

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("imageId")]
    public string? ImageUrl { get; set; }

    [BsonElement("minecraftId")]
    public string? MinecraftId { get; set; }

    [BsonElement("amount")]
    public required int Amount { get; set; }

    [BsonElement("price")]
    public required int Price { get; set; }
}