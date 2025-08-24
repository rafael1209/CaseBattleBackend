using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Interfaces;

public interface ICellRepository
{
    Task<BranchCell> CreateCell(BranchCell cell);
    Task<BranchCell?> GetCellById(ObjectId id);
    Task<List<BranchCell>> GetCellsByBranchId(ObjectId branchId);
    Task<BranchCell> GetEmptyCell(int minSlots);
    Task UpdateCell(BranchCell cell);
}