using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
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

    public async Task<Banner> Create(Banner banner)
    {
        await _banners.InsertOneAsync(banner);

        return banner;
    }
}