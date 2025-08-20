using CaseBattleBackend.Interfaces;
using Discord;
using Discord.WebSocket;

namespace CaseBattleBackend.Services;

public class DiscordNotificationService(DiscordSocketClient client)
    : IDiscordNotificationService
{
    public async Task SendAsync(Embed embed, ulong channelId, MessageComponent? components = null)
    {
        if (client.GetChannel(channelId) is not IMessageChannel channel) return;

        await channel.SendMessageAsync(text: "<@&1214945168768237638>", embed: embed, components: components);
    }
}
