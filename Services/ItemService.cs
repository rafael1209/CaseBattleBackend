using CaseBattleBackend.Dtos;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using MongoDB.Bson;

namespace CaseBattleBackend.Services;

public class ItemService(
    IItemRepository itemRepository,
    IStorageService storageService,
    IMinecraftAssets minecraftAssets) : IItemService
{
    public async Task<CaseItem> Create(CreateItemRequest request)
    {
        FileDto? file = null;
        if (request.File is not null)
            file = await storageService.UploadFile(request.File, request.File.FileName);

        return await itemRepository.Create(new CaseItem
        {
            Id = ObjectId.GenerateNewId(),
            Name = request.Name,
            Description = request.Description,
            ImageId = file?.Id,
            MinecraftId = request.MinecraftId,
            Amount = request.Amount,
            Price = request.Price,
            Rarity = request.Rarity
        });
    }

    public async Task Delete(string id)
    {
        if (!ObjectId.TryParse(id, out var itemId))
            throw new ArgumentException("Invalid ObjectId format", nameof(id));

        var item = await itemRepository.GetById(itemId) ??
            throw new ArgumentException($"Item with ID {id} not found.", nameof(id));

        if (item.ImageId != null) await storageService.DeleteFile(item.ImageId);

        await itemRepository.Delete(itemId);
    }

    public async Task<List<CaseItemView>> GetItems(int fromPrice = 0, int page = 1, int pageSize = 20)
    {
        var items = await itemRepository.Get(fromPrice, page, pageSize);

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

    public async Task<CaseItem?> GetById(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return null;

        return await itemRepository.GetById(objectId);
    }
}