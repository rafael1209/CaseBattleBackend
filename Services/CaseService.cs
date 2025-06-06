using CaseBattleBackend.Dtos;
using CaseBattleBackend.Enums;
using CaseBattleBackend.Helpers;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using MongoDB.Bson;

namespace CaseBattleBackend.Services;

public class CaseService(
    ICaseRepository caseRepository,
    IItemRepository itemRepository,
    IUserService userService,
    IGameResult gameResult,
    IMinecraftItems minecraftItems,
    IStorageService storageService) : ICaseService
{
    public async Task<Case> Create(CreateCaseRequest caseModel)
    {
        var caseItems = new List<ObjectId>();
        foreach (var itemId in caseModel.Items)
        {
            if (!ObjectId.TryParse(itemId, out var id))
                throw new ArgumentException("Invalid ObjectId format", nameof(itemId));

            var item = await itemRepository.GetById(id);
            if (item == null)
                throw new ArgumentException($"Item with ID {itemId} not found.", nameof(itemId));

            caseItems.Add(id);
        }

        var file = await storageService.UploadFile(caseModel.File, caseModel.File.FileName);

        var newCase = new Case
        {
            Id = ObjectId.GenerateNewId(),
            Name = caseModel.Name,
            Description = caseModel.Description,
            ImageId = file.Id,
            Items = caseItems,
            RtpPercentage = caseModel.RtpPercentage,
            Price = caseModel.Price
        };

        return await caseRepository.Create(newCase);
    }

    public async Task<List<CaseDto>> GetAll(int page = 1, int pageSize = 15)
    {
        var cases = await caseRepository.GetAll(page, pageSize);

        var tasks = cases.Select(async caseDto => new CaseDto
        {
            Id = caseDto.Id.ToString(),
            Name = caseDto.Name,
            Description = caseDto.Description,
            ImageUrl = caseDto.ImageId != null ? await storageService.GetFileUrl(caseDto.ImageId) : null,
            Price = caseDto.Price
        });

        var result = await Task.WhenAll(tasks);
        return result.ToList();
    }

    public async Task<CaseViewDto?> GetById(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            throw new ArgumentException("Invalid ObjectId format", nameof(id));

        var caseData = await caseRepository.GetById(objectId) ?? throw new Exception("Case not found");

        var caseItems = new List<CaseItem>();
        foreach (var itemId in caseData.Items)
            caseItems.Add(await itemRepository.GetById(itemId));

        var caseView = new CaseViewDto
        {
            Id = caseData.Id.ToString(),
            Name = caseData.Name,
            Description = caseData.Description,
            ImageUrl = caseData.ImageId != null ? await storageService.GetFileUrl(caseData.ImageId) : null,
            Items = await GetCaseItems(caseItems, caseData.RtpPercentage, caseData.Price),
            Price = caseData.Price
        };

        return caseView;
    }

    public async Task<List<CaseItemViewDto>> OpenCase(User user, string caseId, int amount = 1, bool isDemo = true)
    {
        if (!ObjectId.TryParse(caseId, out var objectId))
            throw new ArgumentException("Invalid Case ID format.", nameof(caseId));

        if (amount is <= 0 or > 4)
            throw new ArgumentException("Amount must be between 1 and 4.", nameof(amount));

        var caseData = await caseRepository.GetById(objectId)
                       ?? throw new Exception("Case not found.");

        switch (isDemo)
        {
            case false when user.Balance < caseData.Price * amount:
                throw new Exception("Insufficient balance to open the case.");
            case false:
                await userService.UpdateBalance(user.Id, -(caseData.Price * amount));
                break;
        }

        var allCaseItems = new List<CaseItem>();
        foreach (var itemId in caseData.Items)
            allCaseItems.Add(await itemRepository.GetById(itemId));

        var caseItemsWithChances = await GetCaseItems(allCaseItems, caseData.RtpPercentage, caseData.Price);

        var sumOfChances = caseItemsWithChances.Sum(item => item.PercentChance);
        if (Math.Abs(sumOfChances - 100.0) > 0.01)
            throw new ArgumentException($"The sum of drop chances ({sumOfChances:F2}%) is not equal to 100%.");

        var resultItems = new List<CaseItemViewDto>();
        var random = new Random();

        for (var i = 0; i < amount; i++)
        {
            var randomNumber = random.NextDouble() * 100.0;
            double cumulativeChance = 0;

            foreach (var item in caseItemsWithChances.OrderBy(caseItemViewDto => caseItemViewDto.PercentChance))
            {
                cumulativeChance += item.PercentChance;

                if (!(randomNumber <= cumulativeChance))
                    continue;

                resultItems.Add(item);

                await gameResult.SaveResult(user, caseData, item, GameType.Case, caseData.Id);

                break;
            }
        }

        while (resultItems.Count < amount)
            resultItems.Add(caseItemsWithChances.OrderByDescending(i => i.PercentChance).First());

        if (!isDemo)
            await userService.AddToInventory(user.Id, resultItems);

        return resultItems;
    }

    private async Task<List<CaseItemViewDto>> GetCaseItems(List<CaseItem> items, int rtp, int casePrice)
    {
        var cheepItems = await itemRepository.GetTopByMaxPrice(0.5, 10);

        var allItems = new List<CaseItem>(items);
        var existingIds = new HashSet<string>(allItems.Select(i => i.Id.ToString()));
        var newItems = cheepItems.Where(i => !existingIds.Contains(i.Id.ToString()));

        allItems.AddRange(newItems);

        return await CalculateItemDropChancesByRtp(allItems, (double)rtp, (double)casePrice);
    }

    private async Task<Uri?> GetItemImageUrlAsync(CaseItem item)
    {
        if (item.MinecraftId != null)
            return await minecraftItems.GetItemImageAsync(item.MinecraftId);

        if (item.ImageId != null)
            return await storageService.GetFileUrl(item.ImageId);

        return null;
    }

    private async Task<List<CaseItemViewDto>> CalculateItemDropChancesByRtp(List<CaseItem> items, double rtp, double casePrice)
    {
        var targetTotalExpectedValue = casePrice * (rtp / 100.0);

        var minPower = 0.01;
        var maxPower = 5.0;
        var bestPower = 1.0;
        const int iterations = 100;
        const double tolerance = 0.001;

        for (var i = 0; i < iterations; i++)
        {
            var currentPower = (minPower + maxPower) / 2.0;

            double sumWeightedValues = 0;
            double sumWeights = 0;

            foreach (var item in items)
            {
                var weight = Math.Pow(1.0 / item.Price, currentPower);
                sumWeights += weight;
                sumWeightedValues += weight * item.Price;
            }

            if (sumWeights == 0)
                break;

            var currentExpectedValue = sumWeightedValues / sumWeights;

            if (Math.Abs(currentExpectedValue - targetTotalExpectedValue) < tolerance)
            {
                bestPower = currentPower;
                break;
            }

            if (currentExpectedValue < targetTotalExpectedValue)
                maxPower = currentPower;
            else
                minPower = currentPower;

            bestPower = currentPower;
        }

        var finalSumWeights = items.Sum(item => Math.Pow(1.0 / item.Price, bestPower));

        var tasks = items.Select(async item =>
        {
            var imageUrl = await GetItemImageUrlAsync(item);

            var weight = finalSumWeights == 0
                ? 1
                : Math.Pow(1.0 / item.Price, bestPower);

            var percentChance = finalSumWeights == 0
                ? 100.0 / items.Count
                : (weight / finalSumWeights) * 100.0;

            return new CaseItemViewDto
            {
                Id = item.Id.ToString(),
                Name = item.Name,
                Description = item.Description,
                ImageUrl = imageUrl,
                Amount = item.Amount,
                Price = item.Price,
                PercentChance = percentChance,
                Rarity = item.Rarity
            };
        });

        var result = await Task.WhenAll(tasks);
        return result.ToList();
    }

}