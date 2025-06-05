using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Interfaces;

public interface ICaseRepository
{
    Task<Case> Create(Case newCase);
    Task<List<Case>> GetAll(int page = 1, int pageSize = 15);
    Task<Case?> GetById(ObjectId id);
}