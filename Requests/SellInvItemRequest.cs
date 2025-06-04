using Microsoft.AspNetCore.Mvc;

namespace CaseBattleBackend.Requests;

public class SellInvItemRequest
{
    [FromQuery(Name = "quantity")]
    public required int Quantity { get; set; }
}