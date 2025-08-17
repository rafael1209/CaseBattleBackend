namespace CaseBattleBackend.Requests;

public class CreateOrderRequest
{
    public required string ItemId { get; set; }
    public required int Amount { get; set; }
    public required string BranchId { get; set; }
}