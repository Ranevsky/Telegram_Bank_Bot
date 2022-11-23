using Bank.Application.Interfaces;
using Bank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bank.Infrastructure.Services;

public class BankChecker : IBankChecker
{
    private readonly IBankContext _db;
    private readonly ILogger<BankChecker> _logger;

    public BankChecker(IBankContext db, ILogger<BankChecker> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task CheckAsync(List<Domain.Entities.Bank> newBanks, City city)
    {
        _logger.LogTrace("Checking banks");
        var banksInDb = _db.Banks
            .Include(b => b.BestCurrencies)
            // Todo: look, compares from reference, but not name value
            .Include(b => b.Departments.Where(d => d.City == city))
            .ThenInclude(d => d.Currencies)
            .ThenInclude(c => c.Currency)
            .ToList();

        foreach (var bankInDb in banksInDb)
        {
            var newBank = newBanks.FirstOrDefault(b => b.FullName.ToUpper() == bankInDb.FullName.ToUpper());

            // If the new bank is not in the database (so the bank is deleted), delete
            if (newBank is null)
            {
                _db.Banks.Remove(bankInDb);
                _logger.LogWarning("Remove bank '{Name}'", bankInDb.Name);
                continue;
            }

            // Best currencies
            foreach (var bestCurrInDb in bankInDb.BestCurrencies)
            {
                var newBestCurr =
                    newBank.BestCurrencies.FirstOrDefault(c => c.Currency.Name == bestCurrInDb.Currency.Name);

                // If the currency is not found in the database (it means deleted), delete
                if (newBestCurr is null)
                {
                    _db.CurrencyExchange.Remove(bestCurrInDb);
                    _logger.LogWarning("Remove best currencies '{Name}' in bank '{FullName}'",
                        bestCurrInDb.Currency.Name,
                        bankInDb.FullName);
                    continue;
                }

                if (bestCurrInDb.Buy != newBestCurr.Buy)
                {
                    _logger.LogInformation(
                        "Change best currency ({Action}) '{Name}' in bank '{FullName}', before '{BeforeBuy}', after '{AfterBuy}'",
                        "Buy", bestCurrInDb.Currency.Name, bankInDb.FullName, bestCurrInDb.Buy, newBestCurr.Buy);
                    bestCurrInDb.Buy = newBestCurr.Buy;
                }

                if (bestCurrInDb.Sell != newBestCurr.Sell)
                {
                    _logger.LogInformation(
                        "Change best currency ({Action}) '{Name}' in bank '{FullName}', before '{BeforeSell}', after '{AfterSell}'",
                        "Sell", bestCurrInDb.Currency.Name, bankInDb.FullName, bestCurrInDb.Sell, newBestCurr.Sell);
                    bestCurrInDb.Sell = newBestCurr.Sell;
                }

                newBank.BestCurrencies.Remove(newBestCurr);
            }

            // Departments
            foreach (var departmentInDb in bankInDb.Departments)
            {
                var newDepartment = newBank.Departments.FirstOrDefault(d => d.Street == departmentInDb.Street);

                if (newDepartment is null)
                {
                    _db.Departments.Remove(departmentInDb);
                    _logger.LogWarning("Remove department in bank '{FullName}'", bankInDb.FullName);
                    continue;
                }

                foreach (var currencyInDb in departmentInDb.Currencies)
                {
                    var newCurrency =
                        newDepartment.Currencies.FirstOrDefault(c => c.Currency.Name == currencyInDb.Currency.Name);

                    if (newCurrency is null)
                    {
                        _db.CurrencyExchange.Remove(currencyInDb);
                        _logger.LogWarning(
                            "Remove currency '{Name}' in department with id = '{Id}', in bank '{FullName}'",
                            currencyInDb.Currency.Name, departmentInDb.Id, bankInDb.FullName);
                        continue;
                    }

                    if (currencyInDb.Buy != newCurrency.Buy)
                    {
                        _logger.LogInformation(
                            "Change currency ({Action}) '{Name}' in department with id = '{Id}', in bank '{FullName}', before '{Buy}', after '{NewCurrencyBuy}'",
                            "Buy", currencyInDb.Currency.Name, departmentInDb.Id, bankInDb.FullName, currencyInDb.Buy,
                            newCurrency.Buy);
                        currencyInDb.Buy = newCurrency.Buy;
                    }

                    if (currencyInDb.Sell != newCurrency.Sell)
                    {
                        _logger.LogInformation(
                            "Change currency ({Action}) '{Name}' in department with id = '{Id}', in bank '{FullName}', before '{Sell}', after '{NewCurrencySell}'",
                            "Sell", currencyInDb.Currency.Name, departmentInDb.Id, bankInDb.FullName, currencyInDb.Sell,
                            newCurrency.Sell);
                        currencyInDb.Sell = newCurrency.Sell;
                    }

                    newDepartment.Currencies.Remove(newCurrency);
                }

                if (newDepartment.Currencies.Count <= 0)
                {
                    newBank.Departments.Remove(newDepartment);
                }
            }

            // Add best curr
            if (newBank.BestCurrencies.Count > 0)
            {
                foreach (var bestCurrency in newBank.BestCurrencies)
                {
                    bankInDb.BestCurrencies.Add(bestCurrency);
                    _logger.LogInformation("Add best currency '{BestCurrencyName}' in bank '{FullName}'",
                        bestCurrency.Currency.Name, bankInDb.FullName);
                }
            }

            // Add Department
            if (newBank.Departments.Count > 0)
            {
                foreach (var newDepartment in newBank.Departments)
                {
                    var departmentInDb = bankInDb.Departments.FirstOrDefault(d => d.Street == newDepartment.Street);

                    if (departmentInDb is null)
                    {
                        bankInDb.Departments.Add(newDepartment);
                        _logger.LogInformation(
                            "Add department with street = '{NewDepartmentStreet}' in bank '{FullName}'",
                            newDepartment.Street, bankInDb.FullName);
                        continue;
                    }

                    // if (department.Currencies.Count < 0) { continue; }
                    // Add curr
                    foreach (var currency in newDepartment.Currencies)
                    {
                        departmentInDb.Currencies.Add(currency);
                        _logger.LogInformation(
                            "Add currency '{CurrencyName}' in department with id = '{Id}', in bank '{FullName}'",
                            currency.Currency.Name,
                            departmentInDb.Id,
                            bankInDb.FullName);
                    }
                }
            }

            newBanks.Remove(newBank);
        }

        // Add remaining bank
        if (newBanks.Count > 0)
        {
            foreach (var newBank in newBanks)
            {
                await _db.Banks.AddAsync(newBank);
                _logger.LogInformation("Add bank '{NewBankFullName}'", newBank.FullName);
            }
        }
    }
}