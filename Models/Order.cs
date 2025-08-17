using CaseBattleBackend.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CaseBattleBackend.Models;

public class Order
{
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

    [BsonElement("userId")]
    public ObjectId UserId { get; set; }

    [BsonElement("courierId")]
    public ObjectId? CourierId { get; set; }

    [BsonElement("cellId")]
    public ObjectId? CellId { get; set; }

    [BsonElement("item")]
    public required InventoryItem Item { get; set; }

    [BsonElement("price")]
    public decimal Price { get; set; }

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public OrderStatus Status { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}