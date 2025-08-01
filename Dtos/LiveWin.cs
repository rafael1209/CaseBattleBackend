using System.Text.Json.Serialization;

namespace CaseBattleBackend.Dtos;

public class LiveWin
{
    [JsonPropertyName("user")]
    public UserDto User { get; set; }

    [JsonPropertyName("case")]
    public CaseDto Case { get; set; }

    [JsonPropertyName("item")]
    public CaseItemView Item { get; set; }
}