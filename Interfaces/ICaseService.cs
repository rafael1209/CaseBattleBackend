using CaseBattleBackend.Dtos;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;

namespace CaseBattleBackend.Interfaces;

public interface ICaseService
{
    Task<Case> Create(CreateCaseRequest request);
    Task<List<Case>> GetAll();
    Task<CaseViewDto?> GetById(string id);
    Task<CaseItemViewDto> OpenCase(string id);
}