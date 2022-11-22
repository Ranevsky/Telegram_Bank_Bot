using Bank.Application.Interfaces;
using Microsoft.Extensions.Logging;
using TelegramBot.Application.Interfaces;

namespace TelegramBot.Infrastructure.Repositories;

// Todo: Look in ILogger (UserRepository, BankRepository)
public class UnitOfWork : IUnitOfWork
{
    private readonly ITelegramContext _dbTg;
    private readonly ILogger<IUnitOfWork> _uowLogger;

    private bool _disposed;

    public UnitOfWork(
        ITelegramContext dbApp,
        ILogger<UnitOfWork> uowLogger,
        IUserRepository usersRepository,
        ITelegramCurrencyRepository telegramCurrenciesRepository,
        IBankRepository banksRepository,
        IDepartmentRepository departmentsRepository)
    {
        _dbTg = dbApp;

        _uowLogger = uowLogger;

        Users = usersRepository;
        TelegramCurrencies = telegramCurrenciesRepository;
        Banks = banksRepository;
        Departments = departmentsRepository;
    }

    public IUserRepository Users { get; }
    public ITelegramCurrencyRepository TelegramCurrencies { get; }
    public IBankRepository Banks { get; }
    public IDepartmentRepository Departments { get; }


    public bool TelegramDbHasChanged => _dbTg.ChangeTracker.HasChanges();

    public async Task SaveAsync()
    {
        var affectedCount = await _dbTg.SaveChangesAsync(CancellationToken.None);
        _uowLogger.LogInformation("Save in telegram database, {AffectedCount} rows affected", affectedCount);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _dbTg.Dispose();
        }

        _disposed = true;
    }
}