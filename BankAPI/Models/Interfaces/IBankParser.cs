using HtmlAgilityPack;

namespace BankAPI.Models.Interfaces;

public interface IBankParser
{
    /// <exception cref="HtmlParseException"></exception>
    List<Bank> Parse(HtmlDocument document, City city);
}
