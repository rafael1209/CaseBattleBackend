using System.Text.Json.Serialization;

namespace CaseBattleBackend.Models;

public class UpdateConfig
{
    [JsonPropertyName("rtp")]
    public int Rtp { get; set; }
}