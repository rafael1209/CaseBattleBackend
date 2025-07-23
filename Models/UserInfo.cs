namespace CaseBattleBackend.Models;

public class UserInfo
{
    public required string Id { get; set; }
    public required double Balance { get; set; }
    public required string Nickname { get; set; }
    public required string AvatarUrl { get; set; }
    public required int Level { get; set; }
    public required string Permission { get; set; }
}