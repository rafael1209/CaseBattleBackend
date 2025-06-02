using CaseBattleBackend.Dtos;
using CaseBattleBackend.Enums;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using MongoDB.Bson;

namespace CaseBattleBackend.Services;

public class ItemService(IItemRepository itemRepository) : IItemService
{
    public async Task<CaseItem> Create(CreateItemRequest request)
    {
        return await itemRepository.Create(new CaseItem
        {
            Id = ObjectId.GenerateNewId(),
            Name = request.Name,
            Description = request.Description,
            ImageId = null,
            MinecraftId = request.MinecraftId,
            Amount = request.Amount,
            Price = request.Price,
            Rarity = request.Rarity
        });
    }

    public async Task<List<CaseItemViewDto>> GetItems()
    {
        var items = await itemRepository.Get();

        return items.Select(item => new CaseItemViewDto
        {
            Id = item.Id.ToString(),
            Name = item.Name,
            Description = item.Description,
            ImageUrl = item.ImageId != null ? new Uri(item.ImageId) : null,
            Amount = item.Amount,
            Price = item.Price,
            Rarity = item.Rarity
        }).ToList();
    }
}