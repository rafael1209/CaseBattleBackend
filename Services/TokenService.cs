using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CaseBattleBackend.Enums;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models; // Для PermissionLevel
using Microsoft.IdentityModel.Tokens;

namespace CaseBattleBackend.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    private readonly string _secretKey =
        configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Jwt Key configuration is missing.");
    private readonly string _issuer =
        configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt Issuer configuration is missing.");

    public string GenerateToken(string userId, PermissionLevel permission = PermissionLevel.User)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new("permission", permission.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _issuer,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
            Expires = null
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }
}
