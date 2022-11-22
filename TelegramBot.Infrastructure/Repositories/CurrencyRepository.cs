using Bank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TelegramBot.Application.Exceptions;
using TelegramBot.Application.Interfaces;

namespace TelegramBot.Infrastructure.Repositories;

public class TelegramCurrencyRepository : ITelegramCurrencyRepository
{
    private readonly ITelegramContext _db;

    public TelegramCurrencyRepository(ITelegramContext db)
    {
        _db = db;
    }

    public IEnumerable<string> GetAllName()
    {
        IEnumerable<string> names = _db.Currencies
            .AsNoTracking()
            .Select(c => c.Name);

        return names;
    }

    public async Task<Currency> GetAsync(string name)
    {
        var curr = await _db.Currencies
                       .FirstOrDefaultAsync(c => c.Name.ToUpper() == name.ToUpper())
                   ?? throw new CurrencyNotFoundException();

        return curr;
    }
}