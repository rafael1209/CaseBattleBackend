using CaseBattleBackend.Dtos;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Services;

public class CellService(ICellRepository cellRepository) : ICellService
{
    public async Task<BranchCell> CreateCell(BranchCell cell)
    {
        return await cellRepository.CreateCell(cell);
    }

    public async Task<BranchCell?> GetCellById(ObjectId id)
    {
        return await cellRepository.GetCellById(id);
    }

    public async Task<List<BranchCell>> GetCellsByBranchId(ObjectId branchId)
    {
        return await cellRepository.GetCellsByBranchId(branchId);
    }

    public async Task<BranchCell> GetEmptyCell(int minSlots)
    {
        return await cellRepository.GetEmptyCell(minSlots);
    }

    public async Task<CellView> GetCellView(ObjectId id)
    {
        var cell = await GetCellById(id);

        return new CellView
        {
            Id = cell?.Id.ToString() ?? "id",
            Name = cell?.Name ?? "unknown"
        };
    }

    public async Task UpdateCellStatus(ObjectId id)
    {
        var cell = await GetCellById(id) ?? throw new Exception($"Cell with ID {id} not found.");
        cell.IsOccupied = !cell.IsOccupied;
        await cellRepository.UpdateCell(cell);
    }
}