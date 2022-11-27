using Bank.Application.Interfaces;

namespace Bank.Infrastructure.Services;

public class MyFinBankUpdater : BankUpdater
{
    public MyFinBankUpdater(GetBanksFromMyFin getBanks, IBankChecker checker)
        : base(getBanks, checker)
    {
    }
}