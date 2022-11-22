using Bank.Domain.Entities;

namespace TelegramBot.Application.Interfaces;

public interface ITelegramCurrencyRepository
{
    IEnumerable<string> GetAllName();
    Task<Currency> GetAsync(string name);
}