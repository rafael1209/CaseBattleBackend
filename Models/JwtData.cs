using CaseBattleBackend.Enums;

namespace CaseBattleBackend.Models;

public class JwtData
{
    public string Id { get; set; } = string.Empty;
    public PermissionLevel Permission { get; set; } = PermissionLevel.User;
}