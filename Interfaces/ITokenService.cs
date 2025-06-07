using System.Security.Claims;

namespace CaseBattleBackend.Interfaces;

public interface ITokenService
{
    string GenerateToken(string value);
    ClaimsPrincipal? ValidateToken(string token);
}