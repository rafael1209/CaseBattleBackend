using CaseBattleBackend.Dtos;
using CaseBattleBackend.Enums;
using CaseBattleBackend.Models;
using CaseBattleBackend.Requests;
using MongoDB.Bson;

namespace CaseBattleBackend.Interfaces;

public interface IGameResult
{
    Task SaveResult(User user, Case caseData, CaseItemViewDto item, GameType type, ObjectId gameId);
}