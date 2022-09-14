using BankAPI.Models.HtmlDocuments.Interfaces;

using HtmlAgilityPack;

namespace BankAPI.Models.HtmlDocuments;

public abstract class GetterHtmlDocFromUrl : IGetHtmlDocument
{
    public string Url { get; }
    public abstract string? Object { get; set; }

    public HtmlDocument Document
    {
        get
        {
            HtmlWeb web = new();
            HtmlDocument doc = web.Load($"{Url}{Object ?? ""}");
            return doc;
        }
    }

    public GetterHtmlDocFromUrl(string url)
    {
        Url = url;
    }
}
