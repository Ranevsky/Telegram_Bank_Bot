using BankAPI.Context;
using BankAPI.Models.Currencies;
using BankAPI.Models.Interfaces;

using Logger;

using Microsoft.EntityFrameworkCore;

namespace BankAPI.Models;

public class MyFinChecker : IBankChecker
{
    private readonly ILogger _logger;

    public MyFinChecker(ILogger logger)
    {
        _logger = logger;
    }

    public async Task CheckAsync(BankContext db, List<Bank> newBanks, City city)
    {
        List<Bank> banksInDb = db.Banks
                .Include(b => b.BestCurrencies)
                .Include(b => b.Departments.Where(d => d.City == city))
                    .ThenInclude(d => d.Currencies)
                .ToList();

        foreach (Bank bankInDb in banksInDb)
        {
            Bank? newBank = newBanks.FirstOrDefault(b => b.FullName.ToUpper() == bankInDb.FullName.ToUpper());

            // If the new bank is not in the database (so the bank is deleted), delete
            if (newBank == null)
            {
                db.Banks.Remove(bankInDb);
                _logger.Warning($"Remove bank '{bankInDb.Name}'");
                continue;
            }

            // Best currencies
            foreach (Currency? bestCurrInDb in bankInDb.BestCurrencies)
            {
                Currency? newBestCurr = newBank.BestCurrencies.FirstOrDefault(c => c.Name == bestCurrInDb.Name);

                // If the currency is not found in the database (it means deleted), delete
                if (newBestCurr == null)
                {
                    db.Currencies.Remove(bestCurrInDb);
                    _logger.Warning($"Remove best currencies '{bestCurrInDb.Name}' in bank '{bankInDb.FullName}'");
                    continue;
                }

                if (bestCurrInDb.Buy != newBestCurr.Buy)
                {
                    bestCurrInDb.Buy = newBestCurr.Buy;
                    _logger.Info($"Change best currency (Buy) '{bestCurrInDb.Name}' in bank '{bankInDb.FullName}', before '{bestCurrInDb.Buy}', after '{newBestCurr.Buy}'");
                }

                if (bestCurrInDb.Sell != newBestCurr.Sell)
                {
                    bestCurrInDb.Sell = newBestCurr.Sell;
                    _logger.Info($"Change best currency (Sell) '{bestCurrInDb.Name}' in bank '{bankInDb.FullName}', before '{bestCurrInDb.Sell}', after '{newBestCurr.Sell}'");
                }

                newBank.BestCurrencies.Remove(newBestCurr);
            }

            // Departments
            foreach (Department? departmentInDb in bankInDb.Departments)
            {
                Department? newDepartment = newBank.Departments.FirstOrDefault(d => d.Street == departmentInDb.Street);

                if (newDepartment == null)
                {
                    db.Departments.Remove(departmentInDb);
                    _logger.Warning($"Remove department in bank '{bankInDb.FullName}'");
                    continue;
                }

                foreach (Currency? currencyInDb in departmentInDb.Currencies)
                {
                    Currency? newCurrency = newDepartment.Currencies.FirstOrDefault(c => c.Name == currencyInDb.Name);

                    if (newCurrency == null)
                    {
                        db.Currencies.Remove(currencyInDb);
                        _logger.Warning($"Remove currency '{currencyInDb.Name}' in department with id = '{departmentInDb.Id}', in bank '{bankInDb.FullName}'");
                        continue;
                    }

                    if (currencyInDb.Buy != newCurrency.Buy)
                    {
                        currencyInDb.Buy = newCurrency.Buy;
                        _logger.Info($"Change currency (Buy) '{currencyInDb.Name}' in department with id = '{departmentInDb.Id}', in bank '{bankInDb.FullName}', before '{currencyInDb.Buy}', after '{newCurrency.Buy}'");

                    }

                    if (currencyInDb.Sell != newCurrency.Sell)
                    {
                        currencyInDb.Sell = newCurrency.Sell;
                        _logger.Info($"Change currency (Sell) '{currencyInDb.Name}' in department with id = '{departmentInDb.Id}', in bank '{bankInDb.FullName}', before '{currencyInDb.Sell}', after '{newCurrency.Sell}'");
                    }

                    newDepartment.Currencies.Remove(newCurrency);
                }

                if (newDepartment.Currencies.Count < 1)
                {
                    newBank.Departments.Remove(newDepartment);
                }

            }

            // Add best curr
            if (newBank.BestCurrencies.Count > 0)
            {
                foreach (Currency? bestCurrency in newBank.BestCurrencies)
                {
                    bankInDb.BestCurrencies.Add(bestCurrency);
                    _logger.Info($"Add best currency '{bestCurrency.Name}' in bank '{bankInDb.FullName}'");
                }
            }

            // Add curr
            if (newBank.Departments.Count > 0)
            {
                foreach (Department? newDepartment in newBank.Departments)
                {
                    Department? departmentInDb = bankInDb.Departments.FirstOrDefault(d => d.Street == newDepartment.Street);

                    if (departmentInDb == null)
                    {
                        bankInDb.Departments.Add(newDepartment);
                        _logger.Info($"Add department with street = '{newDepartment.Street}' in bank '{bankInDb.FullName}'");
                        continue;
                    }

                    // if (department.Currencies.Count < 0) { continue; }

                    foreach (Currency? currency in newDepartment.Currencies)
                    {
                        departmentInDb.Currencies.Add(currency);
                        _logger.Info($"Add currency '{currency.Name}' in department with id = '{departmentInDb.Id}', in bank '{bankInDb.FullName}'");
                    }
                }
            }

            newBanks.Remove(newBank);
        }

        // Add remaining bank
        if (newBanks.Count > 0)
        {
            foreach (Bank? newBank in newBanks)
            {
                await db.Banks.AddAsync(newBank);
                _logger.Info($"Add bank '{newBank.FullName}'");
            }
        }
    }
}