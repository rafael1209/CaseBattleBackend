using CaseBattleBackend.Models;
using MongoDB.Driver;

namespace CaseBattleBackend.Interfaces;

public interface IMongoDbContext
{
    IMongoCollection<User> UsersCollection { get; }
}