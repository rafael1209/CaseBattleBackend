namespace CaseBattleBackend.Requests;

public class CreateItemRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? MinecraftId { get; set; }
    public int Amount { get; set; }
    public int Price { get; set; }
}