using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using MongoDB.Driver;

namespace CaseBattleBackend.Database;

public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;

    private const string ConstUsersCollection = "users";
    private const string ConstItemsCollection = "items";
    private const string ConstCasesCollection = "cases";
    private const string ConstGameResultsCollection = "gameResults";
    private const string ConstBannerCollection = "banners";

    public MongoDbContext(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetValue<string>("MongoDb:ConnectionString"));
        _database = client.GetDatabase(configuration.GetValue<string>("MongoDb:DatabaseName"));
    }

    public IMongoCollection<User> UsersCollection => _database.GetCollection<User>(ConstUsersCollection);
    public IMongoCollection<CaseItem> ItemsCollection => _database.GetCollection<CaseItem>(ConstItemsCollection);
    public IMongoCollection<Case> CasesCollection => _database.GetCollection<Case>(ConstCasesCollection);
    public IMongoCollection<GameResult> GameResultsCollection => _database.GetCollection<GameResult>(ConstGameResultsCollection);
    public IMongoCollection<Banner> BannersCollection => _database.GetCollection<Banner>(ConstBannerCollection);
}