using CaseBattleBackend.Dtos;
using CaseBattleBackend.Models;
using Discord;

namespace CaseBattleBackend.Helpers;

public static class DiscordEmbedBuilder
{
    public static (Embed embed, MessageComponent components) BuildItemWithdraw(User user, CaseItem item, int amount)
    {
        var title = LanguageHelper.GetLocalizedMessage("NewOrderMessage_Title");
        var body = string.Format(LanguageHelper.GetLocalizedMessage("NewOrderMessage_Body"), item.Name, amount);

        var embed = new EmbedBuilder()
            .WithTitle(title)
            .WithDescription(body)
            .WithColor(Color.Orange)
            .WithCurrentTimestamp()
            .Build();

        var button = new ButtonBuilder()
            .WithLabel("Принять заказ")
            .WithCustomId($"accept_withdraw_{amount}_test")
            .WithStyle(ButtonStyle.Success);

        var component = new ComponentBuilder().WithButton(button).Build();

        return (embed, component);
    }
}