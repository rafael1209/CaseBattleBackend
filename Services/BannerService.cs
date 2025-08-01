using CaseBattleBackend.Dtos;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using MongoDB.Bson;

namespace CaseBattleBackend.Services;

public class BannerService(IBannerRepository bannerRepository, IStorageService storageService) : IBannerService
{
    public async Task<BannerView> GetById(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            throw new ArgumentException("Invalid banner ID format.");

        var banner = await bannerRepository.GetById(objectId);

        return new BannerView
        {
            Id = banner.Id.ToString(),
            ImageUrl = await storageService.GetFileUrl(banner.ImageId),
            Url = banner.Url
        };
    }

    public async Task<List<BannerView>> GetBanners()
    {
        var banners = await bannerRepository.Get();

        var tasks = banners.Select(async b => new BannerView
        {
            Id = b.Id.ToString(),
            ImageUrl = await storageService.GetFileUrl(b.ImageId),
            Url = b.Url
        });

        var bannerViews = await Task.WhenAll(tasks);
        return bannerViews.ToList();
    }

    public async Task<BannerView> CreateBanner(CreateBannerRequest bannerRequest)
    {
        var file = await storageService.UploadFile(bannerRequest.Image, bannerRequest.Image.FileName);

        var banner = await bannerRepository.Create(new Banner
        {
            ImageId = file.Id,
            Url = bannerRequest.Url
        });

        return new BannerView
        {
            Id = banner.Id.ToString(),
            ImageUrl = await storageService.GetFileUrl(file.Id),
            Url = bannerRequest.Url
        };
    }

    public async Task DeleteBanner(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            throw new ArgumentException("Invalid banner ID format.");

        var banner = await bannerRepository.GetById(objectId);
        await storageService.DeleteFile(banner.ImageId);

        await bannerRepository.Delete(objectId);
    }
}