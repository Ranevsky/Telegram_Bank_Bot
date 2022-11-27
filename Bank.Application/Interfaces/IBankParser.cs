using Bank.Application.Exceptions;
using Bank.Application.Models;
using Bank.Domain.Entities;
using HtmlAgilityPack;

namespace Bank.Application.Interfaces;

public interface IBankParser
{
    /// <exception cref="HtmlParseException"></exception>
    BanksWithInternetBanks Parse(
        HtmlDocument document,
        City city,
        Currency[]? currenciesInDb = null,
        bool checkRedirection = true);
}