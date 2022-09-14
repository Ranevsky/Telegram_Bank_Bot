using BankAPI.Context;
using BankAPI.Models.Interfaces;

using HtmlAgilityPack;

namespace BankAPI.Models;

public class Updater : IBankUpdater
{
    public IBankChecker Checker => _checker;
    public IBankParser Parser => _parser;

    private readonly IBankChecker _checker;
    private readonly IBankParser _parser;

    public Updater(IBankParser parser, IBankChecker checker)
    {
        _parser = parser;
        _checker = checker;
    }

    /// <exception cref="HtmlParseException"></exception>
    public async Task UpdateAsync(BankContext db, HtmlDocument document, City city)
    {
        List<Bank> banks = Parser.Parse(document, city);
        await Checker.CheckAsync(db, banks, city);
        city.LastUpdate = DateTime.Now;
    }
}