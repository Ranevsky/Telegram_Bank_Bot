namespace Bank.Application.Exceptions;

public class BankUpdateException : Exception
{
    public BankUpdateException(Exception ex)
        : base(ex.Message, ex)
    {
    }
}