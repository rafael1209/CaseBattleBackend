using CaseBattleBackend.Enums;
using System.Text.Json.Serialization;

namespace CaseBattleBackend.Models;

public class UserInfo
{
    public required string Id { get; set; }
    public required double Balance { get; set; }
    public required string Nickname { get; set; }
    public required Uri AvatarUrl { get; set; }
    public required int Level { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required PermissionLevel? Permission { get; set; }
}