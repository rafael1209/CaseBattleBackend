using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace CaseBattleBackend.Middlewares;

public class RateLimitAttribute(int limit) : ActionFilterAttribute
{
    private static readonly MemoryCache Cache = new(new MemoryCacheOptions());

    public int Limit { get; } = limit;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString();

        if (string.IsNullOrEmpty(ipAddress))
        {
            context.Result = new ContentResult
            {
                StatusCode = (int)HttpStatusCode.Forbidden,
                Content = "IP address could not be determined."
            };
            return;
        }

        var cacheKey = $"RateLimit:{ipAddress}";
        var entry = Cache.GetOrCreate(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
            return new RateLimitEntry();
        }) ?? new RateLimitEntry();

        entry.Count++;

        if (entry.Count > Limit)
        {
            context.Result = new ContentResult
            {
                StatusCode = (int)HttpStatusCode.TooManyRequests,
                Content = "Too many requests. Please try again later."
            };
        }
        else
        {
            Cache.Set(cacheKey, entry, TimeSpan.FromMinutes(1));
        }
    }

    private class RateLimitEntry
    {
        public int Count { get; set; }
    }
}