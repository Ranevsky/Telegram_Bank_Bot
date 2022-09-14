using BankAPI.Models;

namespace BankAPI.Repositories.Interfaces;

public interface IBankRepository
{
    Task UpdateAsync(string cityName, bool checkTime = true);
    Task<List<Bank>> GetBanksByCityAsync(string cityName, bool onUpdate = true);
    Task<List<Bank>> GetBanksAsync();
    Task CheckLocationAsync(string cityName);
    Task CheckLocationAsync(City city);
}