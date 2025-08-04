using CaseBattleBackend.Dtos;
using CaseBattleBackend.Enums;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using CaseBattleBackend.Responses;
using MongoDB.Bson;

namespace CaseBattleBackend.Services;

public class UserService(
    IUserRepository userRepository,
    IItemRepository itemRepository,
    ISpPaymentService paymentService,
    ITokenService tokenService,
    IMinecraftAssets minecraftAssets,
    IStorageService storageService,
    ITransactionService transactionService)
    : IUserService
{
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

    public async Task<UserInfo> GetUserInfo(string userId)
    {
        if (!ObjectId.TryParse(userId, out var objectId))
            throw new ArgumentException("Invalid ObjectId format", nameof(userId));

        var user = await userRepository.GetById(objectId) ?? throw new Exception("User not found.");
        var permission = tokenService.GetUserPermissionFromToken(user.AuthToken);

        var userInfo = new UserInfo
        {
            Id = user.Id.ToString(),
            Balance = user.Balance,
            Nickname = user.Username,
            AvatarUrl = await minecraftAssets.GetAvatarUrlById(user.MinecraftUuid),
            Level = 0,
            Permission = permission
        };

        return userInfo;
    }

    public async Task<bool> UpdateBalance(ObjectId id, double amount)
    {
        return await userRepository.UpdateBalance(id, amount);
    }

    public async Task<List<InventoryItemView>> GetInventoryItems(string userId, int page = 1, int pageSize = 32)
    {
        if (!ObjectId.TryParse(userId, out var userIdObj))
            throw new ArgumentException("Invalid ObjectId format", nameof(userId));

        if (page < 1) page = 1;
        if (pageSize is < 1 or > 100) pageSize = 10;

        var items = await userRepository.GetInventoryItems(userIdObj, page, pageSize);

        var inventoryItems = new List<InventoryItemView>();
        foreach (var item in items)
        {
            var itemData = await itemRepository.GetById(item.Id);
            var inventoryItemView = new InventoryItemView
            {
                Item = new CaseItemView
                {
                    Id = itemData.Id.ToString(),
                    Name = itemData.Name,
                    Description = itemData.Description,
                    ImageUrl = itemData.ImageId is not null
                        ? await storageService.GetFileUrl(itemData.ImageId)
                        : itemData.MinecraftId is not null ? await minecraftAssets.GetItemImageAsync(itemData.MinecraftId) : null,
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

    public async Task AddToInventory(ObjectId userId, List<CaseItemView> items)
    {
        await userRepository.AddToInventory(userId, items.Select(i => ObjectId.Parse(i.Id)).ToList());
    }

    public async Task SellItem(string userId, string itemId, int quantity = 1)
    {
        if (!ObjectId.TryParse(itemId, out var id))
            throw new ArgumentException("Invalid ObjectId format", nameof(itemId));

        if (!ObjectId.TryParse(userId, out var userIdObj))
            throw new ArgumentException("Invalid ObjectId format", nameof(userId));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        var user = await userRepository.GetById(userIdObj)
                   ?? throw new Exception("User not found.");

        var inventoryItem = user.Items.FirstOrDefault(i => i.Id == id);

        if (inventoryItem == null || inventoryItem.Amount < quantity)
            throw new Exception("Item not found in inventory or insufficient quantity.");

        var item = await itemRepository.GetById(id)
                   ?? throw new Exception("Item not found in database");

        await userRepository.RemoveFromInventory(user, id, quantity);

        var totalPrice = item.Price * quantity;
        await userRepository.UpdateBalance(user.Id, totalPrice);
    }

    public async Task Withdraw(string userId, string cardId, int amount)
    {
        if (!ObjectId.TryParse(userId, out var userIdObj))
            throw new ArgumentException("Invalid ObjectId format", nameof(userId));

        var user = await userRepository.GetById(userIdObj)
                   ?? throw new Exception("User not found.");

        if (user.Balance < amount)
            throw new Exception("Insufficient balance.");

        var transaction = await transactionService.CreateTransactionAsync(new Transaction
        {
            UserId = user.Id,
            Type = TransactionType.Withdrawal,
            Amount = amount,
            CardNumber = cardId,
            Status = TransactionStatus.Created
        });

        await userRepository.UpdateBalance(user.Id, -amount);

        await transactionService.UpdateTransactionStatusAsync(transaction.Id, TransactionStatus.Success);

        try
        {
            await paymentService.SendTransaction(cardId, amount);
        }
        catch (Exception)
        {
            await userRepository.UpdateBalance(user.Id, amount);

            await transactionService.UpdateTransactionStatusAsync(transaction.Id, TransactionStatus.Failed);
        }
    }

    public async Task<PaymentResponse> CreatePayment(string userId, int amount)
    {
        if (!ObjectId.TryParse(userId, out var userIdObj))
            throw new ArgumentException("Invalid ObjectId format", nameof(userId));

        var transaction = await transactionService.CreateTransactionAsync(new Transaction
        {
            UserId = userIdObj,
            Type = TransactionType.Deposit,
            Amount = amount,
            Status = TransactionStatus.Created
        });

        try
        {
            return await paymentService.CreatePayment($"{userId}:{transaction.Id}", amount);
        }
        catch (Exception)
        {
            await transactionService.UpdateTransactionStatusAsync(transaction.Id, TransactionStatus.Failed);
            throw new Exception("Payment creation failed.");
        }
    }

    public async Task HandlePayment(PaymentNotification notification, string base64Hash)
    {
        await paymentService.HandlePaymentNotification(notification, base64Hash);

        var data = notification.Data?.Split(':');

        if (Equals(!ObjectId.TryParse(data?[0], out var userId)))
            throw new Exception("Invalid user ID in payment notification.");

        if (Equals(!ObjectId.TryParse(data?[0], out var paymentId)))
            throw new Exception("Invalid transaction ID in payment notification.");

        await transactionService.UpdateTransactionStatusAsync(paymentId, TransactionStatus.Success);

        await UpdateBalance(userId, (int)notification.Amount);
    }

    public async Task SetAccess(JwtData userData, SetAccessRequest request)
    {
        if (!ObjectId.TryParse(request.UserId, out var userIdObj))
            throw new ArgumentException("Invalid ObjectId format", nameof(request.UserId));

        var secondUser = await GetById(userIdObj) ??
                         throw new Exception("User not found.");

        var permissionLevel = tokenService.GetUserPermissionFromToken(secondUser.AuthToken);

        if (permissionLevel >= userData.Permission && userData.Permission != PermissionLevel.Owner)
            throw new UnauthorizedAccessException("You cannot set a higher permission level than your own.");

        var authToken = tokenService.GenerateToken(request.UserId, request.Permission);

        await userRepository.UpdateAuthToken(userIdObj, authToken);
    }
}