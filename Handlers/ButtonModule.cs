using CaseBattleBackend.Interfaces;
using Discord;
using Discord.Interactions;

namespace CaseBattleBackend.Handlers;

public class ButtonModule(IButtonService buttonService, IUserService userService, IBranchService branchService, ICellService cellService) : InteractionModuleBase<SocketInteractionContext>
{
    [ComponentInteraction("accept_order_*", true)]
    public async Task HandleAcceptWithdraw(string orderId)
    {
        var order = await buttonService.AcceptOrder(orderId, Context.User.Id);
        var user = await userService.GetById(order.UserId);

        await DeferAsync(ephemeral: true);

        var completeButton = new ButtonBuilder()
            .WithLabel("Выполнил")
            .WithCustomId($"complete_{order.Id}")
            .WithStyle(ButtonStyle.Success);

        var cancelButton = new ButtonBuilder()
            .WithLabel("Отменить")
            .WithCustomId($"cancel_{order.Id}")
            .WithStyle(ButtonStyle.Danger);

        await ModifyOriginalResponseAsync(msg =>
        {
            msg.Content = $"✅ Заказ принят курьером <@{Context.User.Id}>.\nДля игрока ||`{user.Username}`||";
            msg.Components = new ComponentBuilder()
                .WithButton(completeButton)
                .WithButton(cancelButton)
                .Build();
        });
    }

    [ComponentInteraction("complete_*", true)]
    public async Task CompleteOrder(string orderId)
    {
        var order = await buttonService.CompleteOrder(orderId, Context.User.Id);
        var user = await userService.GetById(order.UserId);
        var cell = await cellService.GetCellById(order.CellId);
        var branch = await branchService.GetBranchByIdAsync(cell.BranchId);

        await DeferAsync(ephemeral: true);

        await ModifyOriginalResponseAsync(msg =>
        {
            msg.Content =
                $"✅ Заказ выполнен курьером <@{Context.User.Id}>.\nИгрок {user.Username}\nФилиал `{branch.Name}`({branch.Coordinates.Nether.Color.ToString()} {branch.Coordinates.Nether.Distance})\nКлетка `{cell.Name}`";
            msg.Components = new ComponentBuilder().Build();
        });
    }

    [ComponentInteraction("cancel_*", true)]
    public async Task CancelOrder(string orderId)
    {
        var order = await buttonService.CancelOrder(orderId, Context.User.Id);
        var user = await userService.GetById(order.UserId);

        await DeferAsync(ephemeral: true);

        await ModifyOriginalResponseAsync(msg =>
        {
            msg.Content = $"❌ Заказ отменён курьером <@{Context.User.Id}>.\nДля игрока ||`{user.Username}`||";
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