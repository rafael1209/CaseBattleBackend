namespace CaseBattleBackend.Interfaces;

public interface IMinecraftAssets
{
    Task<Uri> GetItemImageAsync(string minecraftId);
    Task<Uri> GetAvatarUrlById(string minecraftUuid);
}