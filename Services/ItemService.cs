using CaseBattleBackend.Dtos;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using MongoDB.Bson;

namespace CaseBattleBackend.Services;

public class ItemService(IItemRepository itemRepository, IStorageService storageService, IMinecraftAssets minecraftAssets) : IItemService
{
    public async Task<CaseItem> Create(CreateItemRequest request)
    {
        var file = await storageService.UploadFile(request.File, request.File.FileName);

        return await itemRepository.Create(new CaseItem
        {
            Id = ObjectId.GenerateNewId(),
            Name = request.Name,
            Description = request.Description,
            ImageId = file.Id,
            MinecraftId = request.MinecraftId,
            Amount = request.Amount,
            Price = request.Price,
            Rarity = request.Rarity
        });
    }

    public async Task<List<CaseItemView>> GetItems()
    {
        var items = await itemRepository.Get();

        var itemDtos = await Task.WhenAll(items.Select(async item => new CaseItemView
        {
            Id = item.Id.ToString(),
            Name = item.Name,
            Description = item.Description,
            ImageUrl = item.ImageId != null
                ? await storageService.GetFileUrl(item.ImageId) : item.MinecraftId != null
                    ? await minecraftAssets.GetItemImageAsync(item.MinecraftId) : null,
            Amount = item.Amount,
            Price = item.Price,
            Rarity = item.Rarity
        }));

        return itemDtos.ToList();
    }
}