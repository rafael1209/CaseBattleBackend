using CaseBattleBackend.Interfaces;

namespace CaseBattleBackend.Helpers;

public class MinecraftItems(IConfiguration configuration) : IMinecraftItems
{
    private readonly string _assetsUrl =
        configuration["Minecraft:ItemUrl"] ?? throw new InvalidOperationException("Minecraft assets configuration is missing.");

    public Task<Uri> GetItemImageAsync(string minecraftId)
    {
        return Task.FromResult(new Uri(string.Format(_assetsUrl, minecraftId)));
    }
}