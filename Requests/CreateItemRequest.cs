using CaseBattleBackend.Enums;

namespace CaseBattleBackend.Requests;

public class CreateItemRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? MinecraftId { get; set; }
    public IFormFile? File { get; set; }
    public Rarity Rarity { get; set; }
    public int Amount { get; set; }
    public double Price { get; set; }
}