namespace Bank.Application.Exceptions;

public class HtmlParseNotFoundException : HtmlParseException
{
    public HtmlParseNotFoundException(string xPath)
        : base($"Not found node with XPath = '{xPath.Replace("./", "/")}'")
    {
    }

    public HtmlParseNotFoundException(string xPath, string attribute)
        : base($"Not found attribute = '{attribute}' in XPath = '{xPath}'")
    {
    }
}