using CaseBattleBackend.Models;

namespace CaseBattleBackend.Interfaces;

public interface IGameResultRepository
{
    Task<GameResult> SaveResult(GameResult gameResult);
    Task<List<GameResult>> GetGameHistory(int limit = 12);
}