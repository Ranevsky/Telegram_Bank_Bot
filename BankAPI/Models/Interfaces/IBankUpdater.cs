using BankAPI.Context;

using HtmlAgilityPack;

namespace BankAPI.Models.Interfaces;

public interface IBankUpdater
{
    /// <exception cref="HtmlParseException"></exception>
    Task UpdateAsync(BankContext db, HtmlDocument document, City city);
}
