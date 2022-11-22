using Bank.Domain.Entities;

namespace Bank.Application.Interfaces;

public interface IBankChecker
{
    Task CheckAsync(List<Domain.Entities.Bank> newBanks, City city);
}