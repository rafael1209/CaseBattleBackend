using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Interfaces;

public interface IBranchRepository
{
    Task<List<Branch>> GetAllBranchesAsync();
    Task<Branch?> GetBranchByIdAsync(ObjectId branchId);
    Task<Branch> CreateBranchAsync(Branch branch);
    Task<bool> UpdateBranchAsync(Branch branch);
    Task<bool> DeleteBranchAsync(ObjectId branchId);
}