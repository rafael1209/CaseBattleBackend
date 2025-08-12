namespace CaseBattleBackend.Services;

public interface IDiscordNotificationService
{
    Task SendWithdrawRequestAsync(string userName, int amount, string cardId);
}
