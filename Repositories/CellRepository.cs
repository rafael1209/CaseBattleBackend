using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CaseBattleBackend.Repositories;

public class CellRepository(IMongoDbContext context) : ICellRepository
{
    private readonly IMongoCollection<BranchCell> _branches = context.BranchCellsCollection;

    public async Task<List<BranchCell>> GetAllBranchesAsync()
    {
        var branchesCursor = await _branches.FindAsync(_ => true);

        return await branchesCursor.ToListAsync();
    }

    public async Task<BranchCell?> GetBranchByIdAsync(ObjectId branchId)
    {
        var filter = Builders<BranchCell>.Filter.Eq(b => b.Id, branchId);

        return await _branches.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<BranchCell> CreateBranchAsync(BranchCell branch)
    {
        await _branches.InsertOneAsync(branch);

        return branch;
    }

    public async Task<bool> UpdateBranchAsync(BranchCell branch)
    {
        var filter = Builders<BranchCell>.Filter.Eq(b => b.Id, branch.Id);
        var result = await _branches.ReplaceOneAsync(filter, branch);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteBranchAsync(ObjectId branchId)
    {
        var filter = Builders<BranchCell>.Filter.Eq(b => b.Id, branchId);
        var result = await _branches.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }
}