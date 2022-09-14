using BankAPI.Context;

namespace BankAPI.Models.Interfaces;

public interface IBankChecker
{
    Task CheckAsync(BankContext db, List<Bank> newBanks, City city);
}
