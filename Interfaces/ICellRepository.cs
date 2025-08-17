using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Interfaces;

public interface ICellRepository
{
    Task<List<BranchCell>> GetAllBranchesAsync();
    Task<BranchCell?> GetBranchByIdAsync(ObjectId branchId);
    Task<BranchCell> CreateBranchAsync(BranchCell branch);
    Task<bool> UpdateBranchAsync(BranchCell branch);
    Task<bool> DeleteBranchAsync(ObjectId branchId);
}