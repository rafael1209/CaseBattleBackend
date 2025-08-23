using CaseBattleBackend.Dtos;
using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Interfaces;

public interface ICellService
{
    Task<BranchCell> CreateCell(BranchCell cell);
    Task<BranchCell?> GetCellById(ObjectId id);
    Task<List<BranchCell>> GetCellsByBranchId(ObjectId branchId);
    Task<BranchCell> GetEmptyCell(int minSlots);
    Task<CellView> GetCellView(ObjectId id);
}