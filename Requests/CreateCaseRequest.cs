namespace CaseBattleBackend.Requests;

public class CreateCaseRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required IFormFile File { get; set; }
    public required List<string> Items { get; set; } = [];
    public required int Price { get; set; }
    public int RtpPercentage { get; set; }
}