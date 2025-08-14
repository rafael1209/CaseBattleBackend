using Discord;
using Discord.Interactions;
using CaseBattleBackend.Interfaces;

namespace CaseBattleBackend.Handlers;

public class ButtonModule(IButtonService buttonService) : InteractionModuleBase<SocketInteractionContext>
{
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
    public async Task HandleAcceptWithdraw(string orderId)
    {
        var order = await buttonService.GetItemByOrder(orderId);

        await DeferAsync(ephemeral: true);

        await ModifyOriginalResponseAsync(msg =>
        {
            msg.Content = $"✅ Заказ принят курьером <@{Context.User.Id}>.";
            msg.Components = new ComponentBuilder().Build();
        });
    }
}