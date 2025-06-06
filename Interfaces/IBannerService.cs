using CaseBattleBackend.Dtos;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;

namespace CaseBattleBackend.Interfaces;

public interface IBannerService
{
    Task<List<BannerView>> GetBanners();
    Task<BannerView> CreateBanner(CreateBannerRequest bannerRequest);
}