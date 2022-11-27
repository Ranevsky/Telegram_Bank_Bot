namespace Bank.Application.Exceptions;

public class HtmlParseRedirectException : HtmlParseException
{
    public HtmlParseRedirectException()
        : base("A redirect has occurred")
    {
    }
}