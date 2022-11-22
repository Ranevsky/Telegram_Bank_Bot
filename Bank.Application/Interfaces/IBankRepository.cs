using Bank.Domain.Entities;

namespace Bank.Application.Interfaces;

public interface IBankRepository
{
    Task<List<Domain.Entities.Bank>> GetBanksWithCityAsync(string cityName, bool onUpdate = true);
    Task<List<Domain.Entities.Bank>> GetBanksAsync();
    Task CheckLocationAsync(string cityName);
    Task CheckLocationAsync(City city);
    IEnumerable<City> GetCities(bool tracking = false);
}