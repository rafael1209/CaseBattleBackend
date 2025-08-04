using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using CaseBattleBackend.Enums;

namespace CaseBattleBackend.Models;

public class Transaction
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("userId")]
    public ObjectId UserId { get; set; }

    [BsonElement("type")]
    [BsonRepresentation(BsonType.String)]
    public TransactionType Type { get; set; }

    [BsonElement("amount")]
    public int Amount { get; set; }

    [BsonElement("cardNumber")]
    public string? CardNumber { get; set; }

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public TransactionStatus Status { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}