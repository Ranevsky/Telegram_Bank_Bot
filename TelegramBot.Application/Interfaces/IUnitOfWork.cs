using Bank.Application.Interfaces;

namespace TelegramBot.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ITelegramCurrencyRepository TelegramCurrencies { get; }
    IBankRepository Banks { get; }
    IDepartmentRepository Departments { get; }
    bool TelegramDbHasChanged { get; }

    Task SaveAsync();
}