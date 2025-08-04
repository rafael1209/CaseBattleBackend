using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CaseBattleBackend.Repositories;

public class BannerRepository(IMongoDbContext context) : IBannerRepository
{
    private readonly IMongoCollection<Banner> _banners = context.BannersCollection;

    public async Task<List<Banner>> Get()
    {
        var bannersCursor = await _banners.FindAsync(_ => true);

        return await bannersCursor.ToListAsync();
    }

    public async Task<Banner> GetById(ObjectId id)
    {
        var filter = Builders<Banner>.Filter.Eq(b => b.Id, id);
        var banner = await _banners.Find(filter).FirstOrDefaultAsync();
        if (banner == null)
            throw new Exception("Banner not found.");

        return banner;
    }

    public async Task<Banner> Create(Banner banner)
    {
        await _banners.InsertOneAsync(banner);

        return banner;
    }

    public async Task Delete(ObjectId id)
    {
        var filter = Builders<Banner>.Filter.Eq(b => b.Id, id);
        var result = await _banners.DeleteOneAsync(filter);
        if (result.DeletedCount == 0)
        {
            throw new Exception("Banner not found or already deleted.");
        }
    }

    public async Task IncrementClickCount(ObjectId id)
    {
        var filter = Builders<Banner>.Filter.Eq(b => b.Id, id);
        var update = Builders<Banner>.Update.Inc(b => b.ClickCount, 1);
        var result = await _banners.UpdateOneAsync(filter, update);
        
        if (result.MatchedCount == 0)
        {
            throw new Exception("Banner not found.");
        }
    }
}