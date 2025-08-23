using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CaseBattleBackend.Repositories;

public class BranchRepository(IMongoDbContext context) : IBranchRepository
{
    private readonly IMongoCollection<Branch> _branches = context.BranchesCollection;

    public async Task<List<Branch>> GetAllBranchesAsync()
    {
        var branchesCursor = await _branches.FindAsync(_ => true);
        return await branchesCursor.ToListAsync();
    }

    public async Task<Branch?> GetBranchByIdAsync(ObjectId branchId)
    {
        var filter = Builders<Branch>.Filter.Eq(b => b.Id, branchId);
        return await _branches.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Branch> CreateBranchAsync(Branch branch)
    {
        await _branches.InsertOneAsync(branch);
        return branch;
    }

    public Task<bool> UpdateBranchAsync(Branch branch)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteBranchAsync(ObjectId branchId)
    {
        throw new NotImplementedException();
    }
}