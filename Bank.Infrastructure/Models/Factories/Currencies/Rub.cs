using Bank.Domain.Entities;

namespace Bank.Infrastructure.Models.Factories.Currencies;

public class Rub : Currency
{
    public const string CurrName = "RUB";

    public Rub()
    {
        Name = CurrName;
    }
}