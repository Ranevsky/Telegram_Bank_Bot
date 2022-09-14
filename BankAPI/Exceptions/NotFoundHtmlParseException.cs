namespace BankAPI.Exceptions;

public class NotFoundHtmlParseException : HtmlParseException
{
    public NotFoundHtmlParseException(string xPath)
     : base($"Not found node with XPath = '{xPath.Replace("./", "/")}'")
    {

    }

    public NotFoundHtmlParseException(string xPath, string attribute)
        : base($"Not found attribute = '{attribute}' in XPath = '{xPath}'")
    {

    }
}
