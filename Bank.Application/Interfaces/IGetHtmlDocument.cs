using HtmlAgilityPack;

namespace Bank.Application.Interfaces;

public interface IGetHtmlDocument
{
    public string Url { get; }
    public string? Object { get; set; }
    HtmlDocument Document { get; }
}