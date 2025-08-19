using CaseBattleBackend.Helpers;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Bson;
using System.Globalization;
using System.Text.Json;

namespace CaseBattleBackend.Services;

public class AuthorizeService(IConfiguration configuration, ITokenService tokenService, IUserService userService)
    : IAuthorizeService
{
    private readonly string _miniAppToken = configuration["SPWorlds:MiniAppToken"] ??
                                            throw new Exception("MiniAppToken configuration is missing.");

    public async Task<string> AuthorizeUser(JsonElement body)
    {
        var properties = new Dictionary<string, object?>();

        foreach (var prop in body.EnumerateObject())
        {
            var value = prop.Value;

            properties[prop.Name] = value.ValueKind switch
            {
                JsonValueKind.String => value.GetString(),
                JsonValueKind.Number => value.TryGetInt64(out var l) ? l.ToString() : value.GetDouble().ToString(CultureInfo.InvariantCulture),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                JsonValueKind.Array => string.Join(",", value.EnumerateArray().Select(v =>
                    v.ValueKind == JsonValueKind.String ? v.GetString() : v.GetRawText())),
                JsonValueKind.Object => "[object Object]",
                _ => value.GetRawText()
            };
        }

        var isValidate = SpValidate.CheckUser(properties, _miniAppToken);

        if (!isValidate)
            throw new Exception("User validation failed.");

        if (!properties.TryGetValue("minecraftUUID", out var uuidObj) || uuidObj is not string minecraftUuid ||
            string.IsNullOrWhiteSpace(minecraftUuid))
            throw new Exception("Missing or invalid minecraftUUID");

        if (!properties.TryGetValue("username", out var usernameObj) || usernameObj is not string username ||
            string.IsNullOrWhiteSpace(username))
            throw new Exception("Missing or invalid username");

        var userId = ObjectId.GenerateNewId();

        var user = await userService.TryGetByMinecraftUuid(minecraftUuid)
                   ?? await userService.Create(new User
                   {
                       Id = userId,
                       AuthToken = tokenService.GenerateToken(userId.ToString()),
                       MinecraftUuid = minecraftUuid,
                       Username = username
                   });

        return user.AuthToken;
    }
}