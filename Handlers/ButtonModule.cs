using Discord;
using Discord.Interactions;
using System.Text.RegularExpressions;

namespace CaseBattleBackend.Handlers;

public class ButtonModule : InteractionModuleBase<SocketInteractionContext>
{
    //TODO: Remove this test module in production
    [SlashCommand("buttons", "Отправить сообщение с кнопками")]
    public async Task SendButtons()
    {
        var button1 = new ButtonBuilder()
            .WithLabel("Кнопка 1")
            .WithCustomId("btn_1")
            .WithStyle(ButtonStyle.Primary);

        var button2 = new ButtonBuilder()
            .WithLabel("Кнопка 2")
            .WithCustomId("btn_2")
            .WithStyle(ButtonStyle.Danger);

        var component = new ComponentBuilder()
            .WithButton(button1)
            .WithButton(button2)
            .Build();

        await RespondAsync("Нажмите кнопку:", components: component);
    }

    [ComponentInteraction("btn_1")]
    public async Task HandleButton1()
    {
        await DeferAsync();
        await FollowupAsync("Вы нажали кнопку 1!");
    }

    [ComponentInteraction("btn_2")]
    public async Task HandleButton2()
    {
        await DeferAsync();
        await FollowupAsync("Вы нажали кнопку 2!");
    }

    [ComponentInteraction("accept_order_*", true)]
    public async Task HandleAcceptWithdraw(string customId)
    {
        var match = Regex.Match(customId, @"(\d+)");
        if (!match.Success)
        {
            await RespondAsync("Ошибка: неверный формат кнопки.", ephemeral: true);
            return;
        }
        var userName = match.Groups[1].Value;

        await DeferAsync(ephemeral: true);

        await ModifyOriginalResponseAsync(msg =>
        {
            msg.Content = $"✅ Заказ на вывод для пользователя **{userName}** принят курьером <@{Context.User.Id}>.";
            msg.Components = new ComponentBuilder().Build();
        });
    }
}