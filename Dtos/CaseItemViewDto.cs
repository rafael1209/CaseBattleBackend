namespace CaseBattleBackend.Dtos;

public class CaseItemViewDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Uri? ImageUrl { get; set; }
    public int Amount { get; set; }
    public double Price { get; set; }
    public double PercentChance { get; set; }
}