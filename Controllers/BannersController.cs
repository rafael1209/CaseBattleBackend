using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Middlewares;
using CaseBattleBackend.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CaseBattleBackend.Controllers;

[Route("api/v1/banners")]
public class BannersController(IBannerService bannerService) : Controller
{
    [HttpGet]
    [AuthMiddleware]
    public async Task<IActionResult> Get()
    {
        try
        {
            var banners = await bannerService.GetBanners();

            return Ok(banners);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return NotFound(new { message = e.Message });
        }
    }

    [HttpPost]
    [AuthMiddleware]
    public async Task<IActionResult> Create(CreateBannerRequest bannerRequest)
    {
        try
        {
            var banner = await bannerService.CreateBanner(bannerRequest);

            return Ok(banner);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return NotFound(new { message = e.Message });
        }
    }
}