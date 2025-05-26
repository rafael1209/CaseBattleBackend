using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using MongoDB.Bson;

namespace CaseBattleBackend.Services;

public class CaseService(ICaseRepository caseRepository, IItemRepository itemRepository) : ICaseService
{
    public async Task<Case> Create(CreateCaseRequest caseModel)
    {
        var caseItems = new List<ObjectId>();
        foreach (var itemId in caseModel.Items)
        {
            if (!ObjectId.TryParse(itemId, out var id))
                throw new ArgumentException("Invalid ObjectId format", nameof(itemId));

            var item = await itemRepository.GetById(id);

            caseItems.Add(id);
        }

        var newCase = new Case
        {
            Id = ObjectId.GenerateNewId(),
            Name = caseModel.Name,
            Description = caseModel.Description,
            ImageId = null,
            Items = caseItems,
            RtpPercentage = caseModel.RtpPercentage
        };

        return await caseRepository.Create(newCase);
    }

    public async Task<List<Case>> GetAll()
    {
        return await caseRepository.GetAll();
    }

    public async Task<Case?> GetById(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            throw new ArgumentException("Invalid ObjectId format", nameof(id));

        return await caseRepository.GetById(objectId);
    }
}