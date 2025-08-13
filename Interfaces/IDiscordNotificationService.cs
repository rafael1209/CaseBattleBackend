using Discord;

namespace CaseBattleBackend.Interfaces;

public interface IDiscordNotificationService
{
    Task SendAsync(Embed embed, ulong channelId, MessageComponent? components = null);
}