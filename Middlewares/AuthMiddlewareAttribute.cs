using System.Security.Claims;
using CaseBattleBackend.Enums;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CaseBattleBackend.Middlewares;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthMiddlewareAttribute(PermissionLevel requiredPermission = PermissionLevel.User)
    : Attribute, IFilterFactory
{
    public bool IsReusable => false;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        var tokenService = serviceProvider.GetRequiredService<ITokenService>();
        return new AuthMiddlewareFilter(httpContextAccessor, tokenService, requiredPermission);
    }

    private class AuthMiddlewareFilter(
        IHttpContextAccessor httpContextAccessor,
        ITokenService tokenService,
        PermissionLevel requiredPermission
    ) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                var request = httpContextAccessor.HttpContext!.Request;
                var principal = ValidateTokenFromHeader(request);

                if (principal != null)
                {
                    var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var permissionClaim = principal.FindFirst("permission")?.Value;

                    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(permissionClaim))
                    {
                        context.Result = new UnauthorizedResult();
                        return;
                    }

                    var userPermission = Enum.Parse<PermissionLevel>(permissionClaim);

                    if (userPermission < requiredPermission)
                    {
                        context.Result = new ObjectResult(new { message = "Access Forbidden" })
                        {
                            StatusCode = StatusCodes.Status403Forbidden
                        };

                        return;
                    }

                    httpContextAccessor.HttpContext.Items["@me"] = new JwtData
                    {
                        Id = userId,
                        Permission = userPermission
                    };

                    request.HttpContext.User = principal;
                    await next();
                }
                else
                {
                    context.Result = new UnauthorizedResult();
                }
            }
            catch
            {
                context.Result = new UnauthorizedResult();
            }
        }

        private ClaimsPrincipal? ValidateTokenFromHeader(HttpRequest request)
        {
            request.Headers.TryGetValue("Authorization", out var token);
            var jwt = token.FirstOrDefault();

            return string.IsNullOrEmpty(jwt) ? null : tokenService.ValidateToken(jwt);
        }
    }
}