using CaseBattleBackend.Dtos;
using CaseBattleBackend.Models;
using Discord;

namespace CaseBattleBackend.Helpers;

public static class DiscordEmbedBuilder
{
    public static (Embed embed, MessageComponent components) BuildItemWithdraw(User user, CaseItemView item, int amount)
    {
        var title = LanguageHelper.GetLocalizedMessage("NewOrderMessage_Title");
        var body = string.Format(LanguageHelper.GetLocalizedMessage("NewOrderMessage_Body"), item.Name,
            item.Amount * amount, item.Description ?? "none", item.Price * amount);

        var embed = new EmbedBuilder()
            .WithTitle(title)
            .WithDescription(body)
            .WithThumbnailUrl(item.ImageUrl?.ToString() ?? "https://assets.zaralx.ru/api/v1/minecraft/vanilla/item/barrier/icon")
            .WithColor(Color.Orange)
            .Build();

        var button = new ButtonBuilder()
            .WithLabel("Принять заказ")
            .WithCustomId($"accept_order_{amount}")
            .WithStyle(ButtonStyle.Success);

        return (embed, new ComponentBuilder().WithButton(button).Build());
    }
}