using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Interfaces;

public interface ICaseRepository
{
    Task<Case> Create(Case newCase);
    Task<List<Case>> GetAll();
    Task<Case?> GetById(ObjectId id);
}