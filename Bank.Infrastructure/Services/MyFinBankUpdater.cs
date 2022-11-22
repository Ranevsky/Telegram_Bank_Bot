namespace Bank.Infrastructure.Services;

public class MyFinBankUpdater : BankUpdater
{
    public MyFinBankUpdater(MyFinParser parser, MyFinChecker checker)
        : base(parser, checker)
    {
    }
}