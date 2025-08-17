using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CaseBattleBackend.Models;

public class Branch
{
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

    [BsonElement("name")]
    public required string Name { get; set; }

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("coordinates")]
    public required List<Coordinate> Coordinates { get; set; }

    [BsonElement("imageIds")]
    public List<string> ImageIds { get; set; } = [];

    [BsonElement("cells")]
    public required List<BranchCell> Cells { get; set; } = [];

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }
}