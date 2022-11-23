namespace Bank.Domain.Entities;

public class CurrencyExchange
{
    public int Id { get; set; }
    public Currency Currency { get; set; } = null!;
    public decimal Buy { get; set; }
    public decimal Sell { get; set; }
}