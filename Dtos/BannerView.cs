namespace CaseBattleBackend.Dtos;

public class BannerView
{
    public required string Id { get; set; }
    public required Uri ImageUrl { get; set; }
    public Uri? Url { get; set; }
    public int ClickCount { get; set; }
}