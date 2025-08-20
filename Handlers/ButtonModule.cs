using Discord;
using Discord.Interactions;
using CaseBattleBackend.Interfaces;

namespace CaseBattleBackend.Handlers;

public class ButtonModule(IButtonService buttonService, IUserService userService) : InteractionModuleBase<SocketInteractionContext>
{
    [ComponentInteraction("accept_order_*", true)]
    public async Task HandleAcceptWithdraw(string orderId)
    {
        var order = await buttonService.AcceptOrder(orderId, Context.User.Id);
        var user = await userService.GetById(order.Id);

        await DeferAsync(ephemeral: true);

        var button = new ButtonBuilder()
            .WithLabel("Выполнил")
            .WithCustomId($"complete_{order.Id}")
            .WithStyle(ButtonStyle.Success);

        await ModifyOriginalResponseAsync(msg =>
        {
            msg.Content = $"✅ Заказ принят курьером <@{Context.User.Id}>.\n||```{user.Username}```||";
            msg.Components = new ComponentBuilder().WithButton(button).Build();
        });
    }

    [ComponentInteraction("complete_*", true)]
    public async Task CompleteOrder(string orderId)
    {
        await buttonService.CompleteOrder(orderId, Context.User.Id);

        await DeferAsync(ephemeral: true);

        await ModifyOriginalResponseAsync(msg =>
        {
            msg.Content = $"✅ Заказ выполнен курьером <@{Context.User.Id}>.";
            msg.Components = new ComponentBuilder().Build();
        });
    }

    [SlashCommand("ping", "Отправить сообщение с кнопками")]
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
}