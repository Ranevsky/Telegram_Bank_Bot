using Bank.Domain.Entities;

namespace Bank.Infrastructure.Models.Factories.Currencies;

public class EurToUsd : Currency
{
    public const string CurrName = "EurToUsd";

    public EurToUsd()
    {
        Name = CurrName;
    }
}