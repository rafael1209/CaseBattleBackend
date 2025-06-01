using CaseBattleBackend.Database;
using CaseBattleBackend.Helpers;
using CaseBattleBackend.Interfaces;
using CaseBattleBackend.Repositories;
using CaseBattleBackend.Services;
using Microsoft.OpenApi.Models;

namespace CaseBattleBackend;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "CaseBattle API", Version = "v1" });
            c.AddSecurityDefinition(
                "ApiKey",
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Description =
                        "API Key needed to access the endpoints. API Key must be in the 'Authorization' header. For public endpoints you can didn't use API Key, or just type '0'",
                    Name = "Authorization"
                }
            );
            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" } }, [] }
            });
        });

        services.AddHttpContextAccessor();
        services.AddMemoryCache();

        services.AddSingleton<IMongoDbContext, MongoDbContext>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<ICaseRepository, CaseRepository>();
        services.AddScoped<IGameResultRepository, GameResultRepository>();

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthorizeService, AuthorizeService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<ICaseService, CaseService>();
        services.AddScoped<ISpPaymentService, SpPaymentService>();
        services.AddScoped<IGameResult, GameResultService>();
        services.AddScoped<IMinecraftItems, MinecraftItems>();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowAnyHeader();
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseHsts();
        }

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseWebSockets(new WebSocketOptions { KeepAliveInterval = TimeSpan.FromSeconds(30) });

        app.UseCors("AllowAll");

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute().AllowAnonymous();
            endpoints.MapSwagger();
            endpoints.MapControllers().AllowAnonymous();
        });
    }
}