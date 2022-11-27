using Bank.Application.Interfaces;
using Bank.Application.Models;
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

    public async Task CheckAsync(BanksWithInternetBanks banksWithInternetBanks, City city)
    {
        _logger.LogTrace("Checking banks");

        if (banksWithInternetBanks.Banks.Count > 0)
        {
            var banksInDb = await _db.Banks
                .Include(b => b.Departments.Where(d => d.City.Id == city.Id))
                .ThenInclude(d => d.Currencies)
                .ThenInclude(c => c.Currency)
                .Include(b => b.Departments.Where(d => d.City.Id == city.Id))
                .ThenInclude(d => d.City)
                .ToListAsync();

            var newBanks = banksWithInternetBanks.Banks;
            await BankCheckAsync(banksInDb, newBanks);
        }

        if (banksWithInternetBanks.InternetBanks.Count > 0)
        {
            var internetBanksInDb = await _db.InternetBanks
                .Include(b => b.Currencies)
                .ToListAsync();

            var newInternetBanks = banksWithInternetBanks.InternetBanks;
            await InternetBankCheckAsync(internetBanksInDb, newInternetBanks);
        }

        city.LastUpdate = DateTime.Now;
        await _db.SaveChangesAsync(CancellationToken.None);

        // Local functions
        async Task BankCheckAsync(List<Domain.Entities.Bank> banksInDb, List<Domain.Entities.Bank> newBanks)
        {
            foreach (var bankInDb in banksInDb)
            {
                var newBank = newBanks.FirstOrDefault(b =>
                    string.Equals(b.FullName, bankInDb.FullName, StringComparison.CurrentCultureIgnoreCase));

                // If the new bank is not in the database (so the bank is deleted), delete
                if (newBank is null)
                {
                    var count = await _db.Banks.Include(b => b.Departments.Where(d => d.City.Id != city.Id))
                        .Select(b => new { b.Id, b.Departments })
                        .Where(x => x.Id == bankInDb.Id)
                        .Select(b => b.Departments.Count())
                        .FirstOrDefaultAsync();

                    if (count == 0)
                    {
                        _db.Banks.Remove(bankInDb);
                        _logger.LogWarning("Remove bank '{BankName}'", bankInDb.FullName);
                    }

                    continue;
                }

                // Departments
                DepartmentsCheck(bankInDb, newBank);

                newBanks.Remove(newBank);
            }

            // Add remaining bank
            if (newBanks.Count > 0)
            {
                foreach (var newBank in newBanks)
                {
                    await _db.Banks.AddAsync(newBank);
                    _logger.LogInformation("Add bank '{BankName}'", newBank.FullName);
                }
            }

            // Local function
            void DepartmentsCheck(Domain.Entities.Bank bankInDb, Domain.Entities.Bank newBank)
            {
                foreach (var departmentInDb in bankInDb.Departments)
                {
                    var newDepartment = newBank.Departments.FirstOrDefault(d => d.Street == departmentInDb.Street);

                    if (newDepartment is null)
                    {
                        _db.Departments.Remove(departmentInDb);
                        _logger.LogWarning("Remove department in bank '{BankName}'", bankInDb.FullName);
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
                                "Remove currency '{CurrName}' in department with id = '{DepartmentId}', in bank '{BankName}'",
                                currencyInDb.Currency.Name, departmentInDb.Id, bankInDb.FullName);
                            continue;
                        }

                        if (currencyInDb.Buy != newCurrency.Buy)
                        {
                            _logger.LogInformation(
                                "Change currency ({CurrAction}) '{CurrName}' in department with id = '{DepartmentId}', in bank '{BankName}', before '{BeforeBuy}', after '{AfterBuy}'",
                                "Buy", currencyInDb.Currency.Name, departmentInDb.Id, bankInDb.FullName,
                                currencyInDb.Buy,
                                newCurrency.Buy);
                            currencyInDb.Buy = newCurrency.Buy;
                        }

                        if (currencyInDb.Sell != newCurrency.Sell)
                        {
                            _logger.LogInformation(
                                "Change currency ({CurrAction}) '{CurrName}' in department with id = '{DepartmentId}', in bank '{BankName}', before '{BeforeSell}', after '{AfterSell}'",
                                "Sell", currencyInDb.Currency.Name, departmentInDb.Id, bankInDb.FullName,
                                currencyInDb.Sell,
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
                                "Add department with street = '{DepartmentStreet}' in bank '{BankName}'",
                                newDepartment.Street, bankInDb.FullName);
                            continue;
                        }

                        // if (department.Currencies.Count < 0) { continue; }
                        // Add curr
                        foreach (var currency in newDepartment.Currencies)
                        {
                            departmentInDb.Currencies.Add(currency);
                            _logger.LogInformation(
                                "Add currency '{CurrencyName}' in department with id = '{DepartmentId}', in bank '{BankName}'",
                                currency.Currency.Name,
                                departmentInDb.Id,
                                bankInDb.FullName);
                        }
                    }
                }
            }
        }

        async Task InternetBankCheckAsync(List<InternetBank> internetBanksInDb, List<InternetBank> newInternetBanks)
        {
            foreach (var internetBankInDb in internetBanksInDb)
            {
                var newInternetBank = newInternetBanks.FirstOrDefault(b =>
                    string.Equals(b.FullName, internetBankInDb.FullName, StringComparison.CurrentCultureIgnoreCase));

                // If the new bank is not in the database (so the bank is deleted), delete
                if (newInternetBank is null)
                {
                    // _db.InternetBanks.Remove(internetBankInDb);
                    _logger.LogWarning("Remove bank '{BankName}'", internetBankInDb.FullName);
                    continue;
                }

                foreach (var currInDb in internetBankInDb.Currencies)
                {
                    var newCurr =
                        newInternetBank.Currencies.FirstOrDefault(c => c.Currency.Name == currInDb.Currency.Name);

                    // If the currency is not found in the database (it means deleted), delete
                    if (newCurr is null)
                    {
                        _db.CurrencyExchange.Remove(currInDb);
                        _logger.LogWarning("Remove currency '{CurrName}' in internet bank '{BankName}'",
                            currInDb.Currency.Name,
                            internetBankInDb.FullName);

                        continue;
                    }

                    if (currInDb.Buy != newCurr.Buy)
                    {
                        _logger.LogInformation(
                            "Change currency ({CurrAction}) '{CurrName}' in internet bank '{BankName}', before '{BeforeBuy}', after '{AfterBuy}'",
                            "Buy", currInDb.Currency.Name, internetBankInDb.FullName, currInDb.Buy,
                            newCurr.Buy);

                        currInDb.Buy = newCurr.Buy;
                    }

                    if (currInDb.Sell != newCurr.Sell)
                    {
                        _logger.LogInformation(
                            "Change best currency ({CurrAction}) '{CurrName}' in internet bank '{BankName}', before '{BeforeSell}', after '{AfterSell}'",
                            "Sell", currInDb.Currency.Name, internetBankInDb.FullName, currInDb.Sell,
                            newCurr.Sell);

                        currInDb.Sell = newCurr.Sell;
                    }

                    newInternetBank.Currencies.Remove(newCurr);
                }

                newInternetBanks.Remove(newInternetBank);
            }

            // Add remaining bank
            if (newInternetBanks.Count > 0)
            {
                foreach (var newInternetBank in newInternetBanks)
                {
                    await _db.InternetBanks.AddAsync(newInternetBank);
                    _logger.LogInformation("Add internet bank '{BankName}'", newInternetBank.FullName);
                }
            }
        }
    }
}