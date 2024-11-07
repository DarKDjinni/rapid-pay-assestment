namespace Distillery.RapidPay.Domain.Entities;

public sealed class CreditCard : IEntity
{
    public required string CardNumber { get; set; }
    public required string OwnerName { get; set; }
    public required string Ccv { get; set; }
    public required byte Month { get; set; }
    public required ushort Year { get; set; }
    public decimal CreditLine { get; set; } = 0;
    public decimal CreditSpent { get; set; } = 0;

    public string UserId { get; set; } = null!;

    public decimal Balance => this.CreditLine - this.CreditSpent;
}
