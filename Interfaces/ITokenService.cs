namespace CaseBattleBackend.Interfaces;

public interface ITokenService
{
    string GenerateToken(string value);
}