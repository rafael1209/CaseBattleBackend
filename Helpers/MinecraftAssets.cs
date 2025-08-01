using CaseBattleBackend.Interfaces;

namespace CaseBattleBackend.Helpers;

public class MinecraftAssets(IConfiguration configuration) : IMinecraftAssets
{
    private readonly string _assetsUrl =
        configuration["Minecraft:ItemUrl"] ?? throw new InvalidOperationException("Minecraft assets configuration is missing.");
    private readonly string _avatarUrl =
        configuration["Minecraft:AvatarUrl"] ?? throw new InvalidOperationException("Minecraft assets configuration is missing.");

    public Task<Uri> GetItemImageAsync(string minecraftId)
    {
        return Task.FromResult(new Uri(string.Format(_assetsUrl, minecraftId)));
    }

    public Task<Uri> GetAvatarUrlById(string minecraftUuid)
    {
        return Task.FromResult(new Uri(_avatarUrl + minecraftUuid));
    }
}