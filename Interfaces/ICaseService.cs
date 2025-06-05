using CaseBattleBackend.Dtos;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using MongoDB.Bson;

namespace CaseBattleBackend.Interfaces;

public interface ICaseService
{
    Task<Case> Create(CreateCaseRequest request);
    Task<List<CaseDto>> GetAll(int page = 1, int pageSize = 15);
    Task<CaseViewDto?> GetById(string id);
    Task<List<CaseItemViewDto>> OpenCase(User user, string caseId, int amount = 1, bool isDemo = true);
}