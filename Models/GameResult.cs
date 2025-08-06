using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CaseBattleBackend.Models;

public class GameResult
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public required ObjectId UserId { get; set; }

    [BsonElement("caseId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public required ObjectId CaseId { get; set; }

    [BsonElement("itemId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public required ObjectId ItemId { get; set; }

    [BsonElement("bet")]
    public required double Bet { get; set; }

    [BsonElement("winMoney")]
    public required decimal WinMoney { get; set; }

    [BsonElement("dropChance")]
    public required double DropChance { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}