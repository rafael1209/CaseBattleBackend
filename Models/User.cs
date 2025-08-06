    using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CaseBattleBackend.Models;

public class User
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("minecraftUuid")]
    public required string MinecraftUuid { get; set; }

    [BsonElement("username")]
    public required string Username { get; set; }

    [BsonElement("balance")]
    public decimal Balance { get; set; }

    [BsonElement("inventory")]
    public List<InventoryItem> Inventory { get; set; } = [];

    [BsonElement("authToken")]
    public required string AuthToken { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}