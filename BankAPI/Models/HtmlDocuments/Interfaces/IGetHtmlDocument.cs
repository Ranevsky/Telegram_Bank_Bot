using HtmlAgilityPack;

namespace BankAPI.Models.HtmlDocuments.Interfaces;

public interface IGetHtmlDocument
{
    public string Url { get; }
    public string? Object { get; set; }
    HtmlDocument Document { get; }
}