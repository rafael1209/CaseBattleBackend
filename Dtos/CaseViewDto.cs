namespace CaseBattleBackend.Dtos;

public class CaseViewDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Uri? ImageUrl { get; set; }
    public List<CaseItemViewDto> Items { get; set; } = [];
    public int Price { get; set; }
}