namespace CaseBattleBackend.Models;

public class UserInfo(string id, double balance, string nickname, string uuid, int level)
{
    public string Id { get; set; } = id;
    public double Balance { get; set; } = balance;
    public string Nickname { get; set; } = nickname;
    public string MinecraftUuid { get; set; } = uuid;
    public int Level { get; set; } = level;
}