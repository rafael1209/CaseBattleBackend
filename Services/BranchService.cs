using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Services;

public class BranchService(IBranchRepository branchRepository) : IBranchService
{
    public async Task<List<Branch>> GetAllBranchesAsync()
    {
        var branches = await branchRepository.GetAllBranchesAsync();
        return branches;
    }

    public async Task<Branch?> GetBranchByIdAsync(ObjectId branchId)
    {
        return await branchRepository.GetBranchByIdAsync(branchId);
    }

    public async Task<Branch> CreateBranchAsync(Branch branch)
    {
        return await branchRepository.CreateBranchAsync(branch);
    }

    public async Task<bool> UpdateBranchAsync(Branch branch)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteBranchAsync(ObjectId branchId)
    {
        throw new NotImplementedException();
    }
}