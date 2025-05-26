using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;

namespace CaseBattleBackend.Interfaces;

public interface ICaseService
{
    Task<Case> Create(CreateCaseRequest request);
    Task<List<Case>> GetAll();
    Task<Case?> GetById(string id);
}