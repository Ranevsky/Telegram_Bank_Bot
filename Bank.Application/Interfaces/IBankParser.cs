using Bank.Application.Exceptions;
using Bank.Domain.Entities;
using HtmlAgilityPack;

namespace Bank.Application.Interfaces;

public interface IBankParser
{
    /// <exception cref="HtmlParseException"></exception>
    List<Domain.Entities.Bank> Parse(HtmlDocument document, City city);
}