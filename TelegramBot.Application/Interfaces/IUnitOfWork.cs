using Bank.Application.Interfaces;

namespace TelegramBot.Application.Interfaces;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    ITelegramCurrencyRepository TelegramCurrencies { get; }
    IBankRepository Banks { get; }
    IDepartmentRepository Departments { get; }
    ICityRepository Cities { get; }
    bool TelegramDbHasChanged { get; }

    Task SaveAsync();
}