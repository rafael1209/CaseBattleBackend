namespace CaseBattleBackend.Requests;

public class WithdrawRequest
{
    public required int Amount { get; set; }
    public required string CardId { get; set; }
}