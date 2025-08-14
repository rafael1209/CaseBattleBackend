using CaseBattleBackend.Dtos;
using CaseBattleBackend.Models;
using Discord;

namespace CaseBattleBackend.Helpers;

public static class DiscordEmbedBuilder
{
    public static (Embed embed, MessageComponent components) BuildItemWithdraw(User user, InventoryItemView inventoryItem, Order order)
    {
        var title = LanguageHelper.GetLocalizedMessage("NewOrderMessage_Title");
        var body = string.Format(LanguageHelper.GetLocalizedMessage("NewOrderMessage_Body"), inventoryItem.Item.Name,
            inventoryItem.Item.Amount * inventoryItem.Amount, inventoryItem.Item.Description ?? "none",
            inventoryItem.Item.Price * inventoryItem.Amount);

        var embed = new EmbedBuilder()
            .WithTitle(title)
            .WithDescription(body)
            .WithThumbnailUrl(inventoryItem.Item.ImageUrl?.ToString() ?? "https://assets.zaralx.ru/api/v1/minecraft/vanilla/item/barrier/icon")
            .WithColor(Color.Orange)
            .Build();

        var button = new ButtonBuilder()
            .WithLabel("Принять заказ")
            .WithCustomId($"accept_order_{order.Id}")
            .WithStyle(ButtonStyle.Success);

        return (embed, new ComponentBuilder().WithButton(button).Build());
    }
}