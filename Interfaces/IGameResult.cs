using CaseBattleBackend.Enums;
using MongoDB.Bson;

namespace CaseBattleBackend.Interfaces;

public interface IGameResult
{
    Task SaveResult(ObjectId userId, double bet, double winMoney, GameType type, ObjectId gameId);
}