using System.Text.Json;

namespace CaseBattleBackend.Interfaces;

public interface IAuthorizeService
{
    Task<string> AuthorizeUser(JsonElement body);
}