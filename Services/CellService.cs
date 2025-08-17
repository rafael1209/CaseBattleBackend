using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Services;

public class CellService(ICellRepository cellRepository) : ICellService
{
    public async Task<List<BranchCell>> GetAllBranchesAsync()
    {
        var branches = await cellRepository.GetAllBranchesAsync();
        return branches;
    }

    public async Task<BranchCell?> GetCellById(ObjectId branchId)
    {
        return await cellRepository.GetBranchByIdAsync(branchId);
    }

    public async Task<BranchCell> CreateBranchAsync(BranchCell branch)
    {
        return await cellRepository.CreateBranchAsync(branch);
    }

    public async Task<bool> UpdateBranchAsync(BranchCell branch)
    {
        var existingBranch = await cellRepository.GetBranchByIdAsync(branch.Id);
        if (existingBranch == null)
            return false;

        return await cellRepository.UpdateBranchAsync(branch);
    }

    public async Task<bool> DeleteBranchAsync(ObjectId branchId)
    {
        var existingBranch = await cellRepository.GetBranchByIdAsync(branchId);
        if (existingBranch == null)
            return false;

        return await cellRepository.DeleteBranchAsync(branchId);
    }
}