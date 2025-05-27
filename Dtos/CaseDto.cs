namespace CaseBattleBackend.Dtos;

public class CaseDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Uri? ImageUrl { get; set; }
    public int Price { get; set; }
}