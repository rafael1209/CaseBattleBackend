using CaseBattleBackend.Dtos;
using CaseBattleBackend.Requests;

namespace CaseBattleBackend.Interfaces;

public interface IBannerService
{
    Task<BannerView> GetById(string id);
    Task<List<BannerView>> GetBanners();
    Task<BannerView> CreateBanner(CreateBannerRequest bannerRequest);
    Task DeleteBanner(string id);
}