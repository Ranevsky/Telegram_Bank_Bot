namespace Bank.Infrastructure.Services;

public class MyFinBankUpdater : BankUpdater
{
    public MyFinBankUpdater(MyFinParser parser, BankChecker checker)
        : base(parser, checker)
    {
    }
}