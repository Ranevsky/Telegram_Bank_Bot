using Bank.Application.Exceptions;
using Bank.Domain.Entities;

namespace Bank.Application.Interfaces;

public interface IBankUpdater
{
    /// <exception cref="HtmlParseException"></exception>
    Task UpdateAsync(City city);
}