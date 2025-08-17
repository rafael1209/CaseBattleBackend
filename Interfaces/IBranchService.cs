using CaseBattleBackend.Dtos;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using MongoDB.Bson;

namespace CaseBattleBackend.Interfaces;

public interface IBranchService
{
    Task<List<BranchView>> GetAllBranchesAsync();
    Task<Branch?> GetBranchByIdAsync(ObjectId branchId);
    Task<Branch> CreateBranchAsync(CreateBranchRequest request);
    Task<bool> UpdateBranchAsync(Branch branch);
    Task<bool> DeleteBranchAsync(ObjectId branchId);
}