using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CaseBattleBackend.Models;

public class BranchCell
{
    [BsonId]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

    [BsonElement("name")]
    public required string Name { get; set; }

    [BsonElement("branchId")]
    public ObjectId BranchId { get; set; }

    [BsonElement("isOccupied")]
    public bool IsOccupied { get; set; }

    [BsonElement("orderId")]
    public ObjectId? OrderId { get; set; }

    [BsonElement("slots")]
    public int Slots { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}