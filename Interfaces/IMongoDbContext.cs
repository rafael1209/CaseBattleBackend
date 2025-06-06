using CaseBattleBackend.Models;
using MongoDB.Driver;

namespace CaseBattleBackend.Interfaces;

public interface IMongoDbContext
{
    IMongoCollection<User> UsersCollection { get; }
    IMongoCollection<CaseItem> ItemsCollection { get; }
    IMongoCollection<Case> CasesCollection { get; }
    IMongoCollection<GameResult> GameResultsCollection { get; }
    IMongoCollection<Banner> BannersCollection { get; }
}