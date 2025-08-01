using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CaseBattleBackend.Models;

public class GameResult
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("userId")]
    public ObjectId UserId { get; set; }

    [BsonElement("itemId")]
    public ObjectId ItemId { get; set; }

    [BsonElement("caseId")]
    public ObjectId CaseId { get; set; }

    [BsonElement("bet")]
    public double Bet { get; set; }

    [BsonElement("winMoney")]
    public double WinMoney { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}