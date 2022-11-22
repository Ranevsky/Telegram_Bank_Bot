namespace Bank.Application.Exceptions;

public class HtmlParseException : Exception
{
    public HtmlParseException(string message)
        : base(message)
    {
    }
}