using Bank.Application.Exceptions;
using Bank.Application.Interfaces;
using Bank.Domain.Entities;
using HtmlAgilityPack;

namespace Bank.Infrastructure.Services;

public class BankUpdater : IBankUpdater
{
    public BankUpdater(IBankParser parser, IBankChecker checker)
    {
        Parser = parser;
        Checker = checker;
    }

    // public IBankChecker Checker => _checker;
    public IBankParser Parser { get; }
    public IBankChecker Checker { get; }

    /// <exception cref="HtmlParseException"></exception>
    public async Task UpdateAsync(HtmlDocument document, City city)
    {
        var banks = Parser.Parse(document, city);
        await Checker.CheckAsync(banks, city);
        city.LastUpdate = DateTime.Now;
    }
}