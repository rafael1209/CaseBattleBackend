using CaseBattleBackend.Enums;
using MongoDB.Bson;

namespace CaseBattleBackend.Models;

public class Game
{
    public ObjectId Id { get; set; }
    public GameType Type { get; set; }
}