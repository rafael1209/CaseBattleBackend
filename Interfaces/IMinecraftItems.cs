namespace CaseBattleBackend.Interfaces;

public interface IMinecraftItems
{
    Task<Uri> GetItemImageAsync(string minecraftId);
}