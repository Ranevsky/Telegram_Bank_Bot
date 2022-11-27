namespace Bank.Domain.Entities;

public class InternetBank : BaseBank
{
    public List<CurrencyExchange> Currencies { get; set; } = new();
}