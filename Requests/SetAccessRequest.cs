using CaseBattleBackend.Enums;

namespace CaseBattleBackend.Requests;

public class SetAccessRequest
{
    public string UserId { get; set; } = string.Empty;
    public PermissionLevel Permission { get; set; } = PermissionLevel.User;
}