using Bank.Domain.Entities;

namespace Bank.Infrastructure.Models.Factories.Currencies;

public class Usd : Currency
{
    public const string CurrName = "USD";

    public Usd()
    {
        Name = CurrName;
    }
}