using System.Text.Json;
using Bank.Application.Interfaces;
using Bank.Domain.Entities;
using Base.Domain.Entities;
using Geocoding.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bank.Infrastructure.Repositories;

public class BankRepository : IBankRepository
{
    private readonly IBankContext _db;
    private readonly IGeocodingAsync _getLocation;
    private readonly ILogger<BankRepository> _logger;
    private readonly IUpdateExchangeInformation _updateExchange;

    public BankRepository(
        IBankContext db,
        IUpdateExchangeInformation updateExchange,
        IGeocodingAsync getLocation,
        ILogger<BankRepository> logger)
    {
        _db = db;
        _updateExchange = updateExchange;
        _getLocation = getLocation;
        _logger = logger;
    }

    public async Task<List<Domain.Entities.Bank>> GetBanksWithCityAsync(string cityName, bool onUpdate = true)
    {
        if (onUpdate)
        {
            await _updateExchange.UpdateAsync(cityName);
        }

        return await _db.Banks
            .Include(b => b.Departments.Where(d => d.City.Name == cityName))
            .ThenInclude(d => d.Currencies)
            .Include(b => b.Departments.Where(d => d.City.Name == cityName))
            .ThenInclude(d => d.Location)
            .ToListAsync();
    }

    public async Task CheckLocationAsync(string cityName)
    {
        var city = await _db.Cities
            .Include(c => c.Location)
            .FirstOrDefaultAsync(c => c.Name == cityName);

        if (city is null)
        {
            return;
        }

        await CheckLocationAsync(city);
    }

    /// <exception cref="ArgumentNullException"></exception>
    public async Task CheckLocationAsync(City city)
    {
        if (city is null)
        {
            throw new ArgumentNullException(nameof(city));
        }

        var departments = _db.Departments
            .Where(d => d.City.Id == city.Id)
            .Include(d => d.Location)
            .Where(d => d.Location == null)
            .ToList();

        var isSave = true;
        if (city.Radius is null)
        {
            var locationAndRadius = await _getLocation.GetLocationAndRadiusAsync(city.Name);
            city.Radius = locationAndRadius.Radius;
            city.Location ??= locationAndRadius.Location;
        }
        else if (city.Location is null)
        {
            city.Location = await _getLocation.GetLocationAsync(city.Name);
        }
        else
        {
            isSave = false;
        }

        try
        {
            foreach (var department in departments)
            {
                if (department.Location is not null)
                {
                    continue;
                }

                department.Location = await _getLocation.GetLocationAsync(department.Street);

                var distance = Location.Distance(city.Location, department.Location);
                if (distance >= city.Radius)
                {
                    _logger.LogError(
                        "Department with id = '{DepartmentId}', not located in radius of occurrence = '{CityRadius}', distance = '{Distance}', location json = {LocationJson}",
                        department.Id, city.Radius, distance,
                        JsonSerializer.Serialize(department.Location));

                    department.Location = null;
                }
                else
                {
                    isSave = true;
                }
            }
        }
        finally
        {
            if (isSave)
            {
                await _db.SaveChangesAsync(CancellationToken.None);
            }
        }
    }

    public IEnumerable<City> GetCities(bool tracking = false)
    {
        var query = _db.Cities.Include(c => c.Location);

        var queryTracker = tracking
            ? query.AsTracking()
            : query.AsNoTracking();

        return queryTracker;
    }
}