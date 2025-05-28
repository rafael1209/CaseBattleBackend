using CaseBattleBackend.Dtos;
using CaseBattleBackend.Enums;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Services;

public class UserService(IConfiguration configuration, IUserRepository userRepository, IItemRepository itemRepository)
    : IUserService
{
    private readonly string _avatarBaseUrl =
        configuration["Minecraft:AvatarUrl"] ??
        throw new InvalidOperationException("Minecraft avatarUrl configuration is missing.");

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

    public async Task<bool> UpdateBalance(ObjectId id, double amount)
    {
        return await userRepository.UpdateBalance(id, amount);
    }

    public async Task<List<InventoryItemView>> GetInventoryItems(ObjectId userId, int page = 1, int pageSize = 32)
    {
        if (page < 1) page = 1;
        if (pageSize is < 1 or > 100) pageSize = 10;

        var items = await userRepository.GetInventoryItems(userId, page, pageSize);

        var inventoryItems = new List<InventoryItemView>();
        foreach (var item in items)
        {
            var itemData = await itemRepository.GetById(item.Id);
            var inventoryItemView = new InventoryItemView
            {
                Item = new CaseItemViewDto
                {
                    Id = itemData.Id.ToString(),
                    Name = itemData.Name,
                    Description = itemData.Description,
                    ImageUrl = null, // Assuming image URL is not stored in item data
                    Amount = itemData.Amount,
                    Price = itemData.Price,
                    PercentChance = 100,
                    Rarity = itemData.Rarity,
                },
                Amount = item.Amount
            };
            inventoryItems.Add(inventoryItemView);
        }

        return inventoryItems;
    }

    public async Task AddToInventory(ObjectId userId, List<CaseItemViewDto> items)
    {
        await userRepository.AddToInventory(userId, items.Select(i => ObjectId.Parse(i.Id)).ToList());
    }
}