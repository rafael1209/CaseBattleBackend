using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Services;

public class UserService(IConfiguration configuration, IUserRepository userRepository) : IUserService
{
    private readonly string _avatarBaseUrl =
        configuration["Minecraft:AvatarUrl"] ?? throw new InvalidOperationException("Minecraft avatarUrl configuration is missing.");

    public async Task<User?> TryGetByMinecraftUuid(string minecraftUuid)
    {
        return await userRepository.TryGetByMinecraftUuid(minecraftUuid);
    }

    public async Task<User> Create(User user)
    {
        return await userRepository.Create(user);
    }

    public async Task<User?> GetById(ObjectId id)
    {
        return await userRepository.GetById(id);
    }

    public async Task<UserInfo> GetUserInfo(User user)
    {
        var userInfo = new UserInfo
        {
            Id = user.Id.ToString(),
            Balance = user.Balance,
            Nickname = user.Username,
            AvatarUrl = _avatarBaseUrl + user.MinecraftUuid,
            Level = 0
        };

        return userInfo;
    }
}