namespace Bank.Application.Exceptions;

public class CityNotValidException : Exception
{
    public CityNotValidException(string message)
        : base(message)
    {
    }
}