using Bank.Application.Interfaces;
using HtmlAgilityPack;

namespace Bank.Infrastructure.Services.HtmlDocuments;

public abstract class GetterHtmlDocFromUrl : IGetHtmlDocument
{
    protected GetterHtmlDocFromUrl(string url)
    {
        Url = url;
    }

    public string Url { get; }
    public abstract string? Object { get; set; }

    public virtual HtmlDocument Document
    {
        get
        {
            HtmlWeb web = new();
            var doc = web.Load($"{Url}{Object ?? ""}");
            return doc;
        }
    }
}