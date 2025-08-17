using CaseBattleBackend.Models;

namespace CaseBattleBackend.Requests;

public class CreateBranchRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Coordinate? Coordinates { get; set; }
    public List<IFormFile>? Files { get; set; }
}