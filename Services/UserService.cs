using CaseBattleBackend.Dtos;
using CaseBattleBackend.Enums;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using CaseBattleBackend.Responses;
using MongoDB.Bson;

namespace CaseBattleBackend.Services;

public class UserService(
    IConfiguration configuration,
    IUserRepository userRepository,
    IItemRepository itemRepository,
    ISpPaymentService paymentService)
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

    public async Task SellItem(User user, string itemId)
    {
        if (!ObjectId.TryParse(itemId, out var id))
            throw new ArgumentException("Invalid ObjectId format", nameof(itemId));

        if (user.Items.All(i => i.Id != id))
            throw new Exception("Item not found in inventory.");

        var item = await itemRepository.GetById(id)
                   ?? throw new Exception("Item not found");

        await userRepository.RemoveFromInventory(user, id);

        await userRepository.UpdateBalance(user.Id, item.Price);
    }

    public async Task Withdraw(User user, string cardId, int amount)
    {
        if (user.Balance < amount)
            throw new Exception("Insufficient balance.");

        await userRepository.UpdateBalance(user.Id, -amount);

        await paymentService.SendTransaction(cardId, amount);
    }

    public async Task<PaymentResponse> CreatePayment(User user, int amount)
    {
        return await paymentService.CreatePayment(user, amount);
    }

    public async Task HandlePayment(PaymentNotification notification, string base64Hash)
    {
        await paymentService.HandlePaymentNotification(notification, base64Hash);

        if (Equals(!ObjectId.TryParse(notification.Data, out var id)))
            throw new Exception("Invalid user ID in payment notification.");

        await UpdateBalance(id, (int)notification.Amount);
    }
}