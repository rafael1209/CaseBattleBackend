using CaseBattleBackend.Dtos;
using CaseBattleBackend.Models;
using MongoDB.Bson;

namespace CaseBattleBackend.Interfaces;

public interface IGameResult
{
    Task SaveResult(User user, Case caseData, CaseItemView item, ObjectId gameId);
}