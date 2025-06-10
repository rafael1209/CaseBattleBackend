using System.Security.Claims;
using CaseBattleBackend.Enums;

namespace CaseBattleBackend.Interfaces;

public interface ITokenService
{
    string GenerateToken(string value, PermissionLevel level = PermissionLevel.User);
    ClaimsPrincipal? ValidateToken(string token);
    PermissionLevel? GetUserPermissionFromToken(string token);
}