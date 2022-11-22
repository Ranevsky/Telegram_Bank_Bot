using Bank.Domain.Entities;

namespace Bank.Infrastructure.Models.Factories.Currencies;

public class Usd : Currency
{
    public Usd()
    {
        Name = "USD";
    }
}