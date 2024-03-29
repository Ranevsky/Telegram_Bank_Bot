﻿using Bank.Application.Interfaces;
using Microsoft.Extensions.Logging;
using TelegramBot.Application.Interfaces;

namespace TelegramBot.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ITelegramContext _dbTg;
    private readonly ILogger<IUnitOfWork> _uowLogger;

    public UnitOfWork(
        ITelegramContext dbApp,
        ILogger<UnitOfWork> uowLogger,
        IUserRepository usersRepository,
        ITelegramCurrencyRepository telegramCurrenciesRepository,
        IBankRepository banksRepository,
        IDepartmentRepository departmentsRepository,
        ICityRepository cityRepository)
    {
        _dbTg = dbApp;

        _uowLogger = uowLogger;

        Users = usersRepository;
        TelegramCurrencies = telegramCurrenciesRepository;
        Banks = banksRepository;
        Departments = departmentsRepository;
        Cities = cityRepository;
    }

    public IUserRepository Users { get; }
    public ITelegramCurrencyRepository TelegramCurrencies { get; }
    public IBankRepository Banks { get; }
    public IDepartmentRepository Departments { get; }
    public ICityRepository Cities { get; }


    public bool TelegramDbHasChanged => _dbTg.ChangeTracker.HasChanges();

    public async Task SaveAsync()
    {
        var affectedCount = await _dbTg.SaveChangesAsync(CancellationToken.None);
        _uowLogger.LogInformation("Save in telegram database, {AffectedCount} rows affected", affectedCount);
    }
}