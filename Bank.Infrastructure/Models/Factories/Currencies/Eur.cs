using Bank.Domain.Entities;

namespace Bank.Infrastructure.Models.Factories.Currencies;

public class Eur : Currency
{
    public const string CurrName = "Eur";

    public Eur()
    {
        Name = CurrName;
    }
}