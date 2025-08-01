using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Interfaces;

public interface IBannerRepository
{
    Task<List<Banner>> Get();
    Task<Banner> GetById(ObjectId id);
    Task<Banner> Create(Banner banner);
    Task Delete(ObjectId id);
}