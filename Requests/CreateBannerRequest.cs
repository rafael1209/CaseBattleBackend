namespace CaseBattleBackend.Requests;

public class CreateBannerRequest
{
    public IFormFile Image { get; set; }
    public Uri Url { get; set; }
}