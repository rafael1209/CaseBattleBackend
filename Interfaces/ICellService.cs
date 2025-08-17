using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Interfaces;

public interface ICellService
{
    Task<List<BranchCell>> GetAllBranchesAsync();
    Task<BranchCell?> GetCellById(ObjectId branchId);
    Task<BranchCell> CreateBranchAsync(BranchCell branch);
    Task<bool> UpdateBranchAsync(BranchCell branch);
    Task<bool> DeleteBranchAsync(ObjectId branchId);
}