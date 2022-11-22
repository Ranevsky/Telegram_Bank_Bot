using Bank.Application.Exceptions;
using Bank.Domain.Entities;
using HtmlAgilityPack;

namespace Bank.Application.Interfaces;

public interface IBankUpdater
{
    /// <exception cref="HtmlParseException"></exception>
    Task UpdateAsync(HtmlDocument document, City city);
}