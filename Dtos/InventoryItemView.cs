namespace CaseBattleBackend.Dtos;

public class InventoryItemView
{
    public required CaseItemView Item { get; set; }
    public int Amount { get; set; }
}