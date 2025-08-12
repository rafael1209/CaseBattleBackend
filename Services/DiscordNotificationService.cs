using Discord;
using Discord.WebSocket;

namespace CaseBattleBackend.Services;

public class DiscordNotificationService(DiscordSocketClient client, IConfiguration configuration)
    : IDiscordNotificationService
{
    public async Task SendWithdrawRequestAsync(string userName, int amount, string cardId)
    {
        var channelId = configuration.GetValue<ulong>("Discord:WithdrawChannelId");
        if (client.GetChannel(channelId) is not IMessageChannel channel) return;

        var embed = new EmbedBuilder()
            .WithTitle($"Запрос на вывод средств")
            .WithDescription($"Пользователь: **{userName}**\nСумма: **{amount}**\nКарта: **{cardId}**")
            .WithColor(Color.Orange)
            .WithCurrentTimestamp()
            .Build();

        var button = new ButtonBuilder()
            .WithLabel("Принять заказ")
            .WithCustomId($"accept_withdraw_{userName}_{amount}_{cardId}")
            .WithStyle(ButtonStyle.Success);

        var component = new ComponentBuilder().WithButton(button).Build();

        await channel.SendMessageAsync(embed: embed, components: component);
    }
}
    