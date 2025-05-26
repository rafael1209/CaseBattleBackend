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
            Price = request.Price
        });
    }
}