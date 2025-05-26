using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using System.Text.Json;
using CaseBattleBackend.Helpers;

namespace CaseBattleBackend.Services;

public class AuthorizeService(IConfiguration configuration, ITokenService tokenService, IUserService userService)
    : IAuthorizeService
{
    private readonly string _miniAppToken = configuration["SPWorlds:MiniAppToken"] ??
                                            throw new Exception("MiniAppToken configuration is missing.");

    public async Task<string> AuthorizeUser(JsonElement body)
    {
        var properties = new Dictionary<string, string>();

        foreach (var prop in body.EnumerateObject())
            properties[prop.Name] = prop.Value.ToString();

        var isValidate = SpValidate.CheckUser(properties, _miniAppToken);

        if (!isValidate)
            throw new Exception("User validation failed.");

        var user = await userService.TryGetByMinecraftUuid(properties["minecraftUUID"]) ?? await userService.Create(new User
        {
            AuthToken = tokenService.GenerateToken(properties["discordId"]),
            DiscordId = long.Parse(properties["discordId"]),
            MinecraftUuid = properties["minecraftUUID"],
            Username = properties["username"]
        });

        return user.AuthToken;
    }
}