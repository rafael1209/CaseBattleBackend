using CaseBattleBackend.Enums;
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
    public string? ImageId { get; set; }

    [BsonElement("minecraftId")]
    public string? MinecraftId { get; set; }

    [BsonElement("amount")]
    public required int Amount { get; set; }

    [BsonElement("price")]
    public required decimal Price { get; set; }

    [BsonElement("stackAmount")]
    public double? StackAmount { get; set; }

    [BsonElement("rarity")]
    [BsonRepresentation(BsonType.String)]
    public required Rarity Rarity { get; set; }
}