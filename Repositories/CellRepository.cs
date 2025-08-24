using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CaseBattleBackend.Repositories;

public class CellRepository(IMongoDbContext context) : ICellRepository
{
    private readonly IMongoCollection<BranchCell> _cells = context.BranchCellsCollection;
    public async Task<BranchCell> CreateCell(BranchCell cell)
    {
        await _cells.InsertOneAsync(cell);

        return cell;
    }

    public async Task<BranchCell?> GetCellById(ObjectId id)
    {
        var filter = Builders<BranchCell>.Filter.Eq(c => c.Id, id);
        return await _cells.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<List<BranchCell>> GetCellsByBranchId(ObjectId branchId)
    {
        var filter = Builders<BranchCell>.Filter.Eq(c => c.BranchId, branchId);

        return await _cells.Find(filter).ToListAsync();
    }

    public async Task<BranchCell> GetEmptyCell(int minSlots)
    {
        var filter = Builders<BranchCell>.Filter.And(
            Builders<BranchCell>.Filter.Eq(c => c.IsOccupied, false),
            Builders<BranchCell>.Filter.Gte(c => c.Slots, minSlots)
        );
        var cell = await _cells.Find(filter).FirstOrDefaultAsync() ??
                   throw new Exception("No empty cell found with the required slots.");

        return cell;
    }

    public async Task UpdateCell(BranchCell cell)
    {
        var filter = Builders<BranchCell>.Filter.Eq(c => c.Id, cell.Id);
        await _cells.ReplaceOneAsync(filter, cell);
    }
}