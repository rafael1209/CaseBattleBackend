using CaseBattleBackend.Dtos;
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
            if (item == null)
            {
                throw new ArgumentException($"Item with ID {itemId} not found.", nameof(itemId));
            }

            caseItems.Add(id);
        }

        var newCase = new Case
        {
            Id = ObjectId.GenerateNewId(),
            Name = caseModel.Name,
            Description = caseModel.Description,
            ImageId = null,
            Items = caseItems,
            RtpPercentage = caseModel.RtpPercentage,
            Price = caseModel.Price
        };

        return await caseRepository.Create(newCase);
    }

    public async Task<List<Case>> GetAll()
    {
        return await caseRepository.GetAll();
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
            ImageUrl = caseData.ImageId != null ? new Uri(caseData.ImageId) : null,
            Items = await GetCaseItems(caseItems, caseData.RtpPercentage, caseData.Price),
            Price = caseData.Price
        };

        return caseView;
    }

    public async Task<CaseItemViewDto> OpenCase(string caseId)
    {
        if (!ObjectId.TryParse(caseId, out var objectId))
            throw new ArgumentException("Invalid Case ID format.", nameof(caseId));

        var caseData = await caseRepository.GetById(objectId) ?? throw new Exception("Case not found.");

        var allCaseItems = new List<CaseItem>();
        foreach (var itemId in caseData.Items)
        {
            var item = await itemRepository.GetById(itemId);
            if (item != null)
                allCaseItems.Add(item);
        }

        var caseItemsWithChances = await GetCaseItems(allCaseItems, caseData.RtpPercentage, caseData.Price);

        var sumOfChances = caseItemsWithChances.Sum(item => item.PercentChance);
        if (Math.Abs(sumOfChances - 100.0) > 0.01) // Допустимая погрешность 0.01%
        {
            // Это может быть индикатором проблемы в CalculateItemDropChancesByRtp.
            // Если шансы не суммируются к 100%, алгоритм случайного выбора может работать некорректно.
            throw new ArgumentException($"Сумма процентов выпадения ({sumOfChances:F2}%) не равна 100%. Пересчитайте шансы или проверьте алгоритм.");
        }

        var random = new Random();
        var randomNumber = random.NextDouble() * 100.0; // Число от 0.0 до 99.999...

        double cumulativeChance = 0;
        foreach (var item in caseItemsWithChances.OrderBy(i => i.PercentChance)) // Сортировка необязательна, но может помочь в отладке
        {
            cumulativeChance += item.PercentChance;

            if (randomNumber <= cumulativeChance)
            {
                return item; // Возвращаем выпавший предмет
            }
        }

        Console.WriteLine("Предупреждение: Случайное число не попало ни в один диапазон шансов. Возвращаем предмет с наибольшим шансом.");
        return caseItemsWithChances.OrderByDescending(i => i.PercentChance).First(); // Используем First() так как уверены, что список не пуст
    }

    private async Task<List<CaseItemViewDto>> GetCaseItems(List<CaseItem> items, int rtp, int casePrice)
    {
        var cheepItems = await itemRepository.GetTopByMaxPrice(casePrice - 1, 10);

        var allItems = new List<CaseItem>(items);
        var existingIds = new HashSet<string>(allItems.Select(i => i.Id.ToString()));
        var newItems = cheepItems.Where(i => !existingIds.Contains(i.Id.ToString()));

        allItems.AddRange(newItems);

        return CalculateItemDropChancesByRtp(allItems, (double)rtp, (double)casePrice);
    }

    private List<CaseItemViewDto> CalculateItemDropChancesByRtp(List<CaseItem> items, double rtp, double casePrice)
    {
        var targetTotalExpectedValue = casePrice * (rtp / 100.0);

        var minPower = 0.01;
        var maxPower = 5.0;
        var bestPower = 1.0;
        const int iterations = 100;
        const double tolerance = 0.001;

        for (var i = 0; i < iterations; i++)
        {
            double currentPower = (minPower + maxPower) / 2.0;

            double sumWeightedValues = 0;
            double sumWeights = 0;

            foreach (var item in items)
            {
                double weight = Math.Pow(1.0 / item.Price, currentPower);
                sumWeights += weight;
                sumWeightedValues += weight * item.Price;
            }

            if (sumWeights == 0)
            {
                break;
            }

            var currentExpectedValue = sumWeightedValues / sumWeights;

            if (Math.Abs(currentExpectedValue - targetTotalExpectedValue) < tolerance)
            {
                bestPower = currentPower;
                break;
            }

            if (currentExpectedValue < targetTotalExpectedValue)
            {
                maxPower = currentPower;
            }
            else
            {
                minPower = currentPower;
            }
            bestPower = currentPower;
        }

        double finalSumWeights = 0;
        foreach (var item in items)
        {
            finalSumWeights += Math.Pow(1.0 / item.Price, bestPower);
        }

        if (finalSumWeights == 0)
        {
            return items.Select(item => new CaseItemViewDto
            {
                Id = item.Id.ToString(),
                Name = item.Name,
                Description = item.Description,
                ImageUrl = null,
                Amount = item.Amount,
                Price = item.Price,
                PercentChance = 100.0 / items.Count
            }).ToList();
        }

        var result = items.Select(item =>
        {
            var weight = Math.Pow(1.0 / item.Price, bestPower);
            var percentChance = (weight / finalSumWeights) * 100.0;

            return new CaseItemViewDto
            {
                Id = item.Id.ToString(),
                Name = item.Name,
                Description = item.Description,
                ImageUrl = item.ImageId != null ? new Uri(item.ImageId) : null,
                Amount = item.Amount,
                Price = item.Price,
                PercentChance = percentChance
            };
        }).ToList();

        return result;
    }
}