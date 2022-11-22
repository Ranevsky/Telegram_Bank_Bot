using Bank.Domain.Entities;

namespace Bank.Infrastructure.Models.Factories.Currencies;

public class EurToUsd : Currency
{
    public EurToUsd()
    {
        Name = "EurToUsd";
    }
}