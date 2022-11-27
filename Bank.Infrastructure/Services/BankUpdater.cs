using Bank.Application.Interfaces;
using Bank.Domain.Entities;

namespace Bank.Infrastructure.Services;

public class BankUpdater : IBankUpdater
{
    private readonly IBankChecker _checker;
    private readonly IGetBanks _getBanks;

    public BankUpdater(IGetBanks getBanks, IBankChecker checker)
    {
        _getBanks = getBanks;
        _checker = checker;
    }

    public async Task UpdateAsync(City city)
    {
        var banks = await _getBanks.GetBanksAsync(city);

        await _checker.CheckAsync(banks, city);
    }
}