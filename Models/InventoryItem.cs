using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CaseBattleBackend.Models;

public class InventoryItem(ObjectId id, int amount)
{
    [BsonElement("id")]
    public ObjectId Id { get; set; } = id;
    [BsonElement("amount")]
    public int Amount { get; set; } = amount;
}