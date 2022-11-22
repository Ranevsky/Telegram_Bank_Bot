using Bank.Domain.Entities;

namespace Bank.Infrastructure.Models.Factories.Currencies;

public class Eur : Currency
{
    public Eur()
    {
        Name = "EUR";
    }
}