using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace CaseBattleBackend.Services;

public class DiscordBotHostedService(
    DiscordSocketClient client,
    InteractionService interactionService,
    IServiceProvider services,
    IConfiguration configuration)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        client.Log += LogAsync;
        interactionService.Log += LogAsync;

        client.Ready += ClientReady;
        client.InteractionCreated += InteractionCreated;

        var token = configuration["Discord:BotToken"];
        if (string.IsNullOrWhiteSpace(token))
            throw new Exception("Discord bot token is not configured!");

        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await client.StopAsync();
        await client.DisposeAsync();
    }

    private async Task ClientReady()
    {
        await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        await interactionService.RegisterCommandsGloballyAsync();
        Console.WriteLine("Discord bot is connected and commands are registered.");
    }

    private async Task InteractionCreated(SocketInteraction interaction)
    {
        var ctx = new SocketInteractionContext(client, interaction);
        await interactionService.ExecuteCommandAsync(ctx, services);
    }

    private Task LogAsync(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}