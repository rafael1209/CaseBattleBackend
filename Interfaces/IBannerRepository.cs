using CaseBattleBackend.Models;

namespace CaseBattleBackend.Interfaces;

public interface IBannerRepository
{
    Task<List<Banner>> Get();
    Task<Banner> Create(Banner banner);
}