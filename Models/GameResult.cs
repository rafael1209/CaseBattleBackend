using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CaseBattleBackend.Models;

public class GameResult
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("userId")]
    public ObjectId UserId { get; set; }

    [BsonElement("gameType")]
    public Game Game { get; set; } = new();

    [BsonElement("bet")]
    public double Bet { get; set; }

    [BsonElement("winMoney")]
    public double WinMoney { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}