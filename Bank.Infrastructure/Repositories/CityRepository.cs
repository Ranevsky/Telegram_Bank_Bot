using Bank.Application.Interfaces;
using Bank.Domain.Entities;
using Bank.Infrastructure.Extensions;
using Geocoding.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bank.Infrastructure.Repositories;

public class CityRepository : ICityRepository
{
    private readonly IBankContext _db;
    private readonly IGeocodingAsync _geocoding;
    private readonly ILogger<CityRepository> _logger;

    public CityRepository(IBankContext db, IGeocodingAsync geocoding, ILogger<CityRepository> logger)
    {
        _db = db;
        _geocoding = geocoding;
        _logger = logger;
    }

    public async Task<City> CreateIfNotExistAsync(string name)
    {
        name = name.Capitalize();
        var city = await GetAsync(name);

        if (city is not null)
        {
            return city;
        }

        var locationAndRadius = await _geocoding.GetLocationAndRadiusAsync(name);
        city = new City
        {
            Name = name,
            Location = locationAndRadius.Location,
            Radius = locationAndRadius.Radius
        };

        await _db.Cities.AddAsync(city);
        await _db.SaveChangesAsync(CancellationToken.None);

        _logger.LogInformation("Added city '{CityName}'", city.Name);

        return city;
    }

    private async Task<City?> GetAsync(string name)
    {
        var city = await _db.Cities
            .Include(c => c.Location)
            .FirstOrDefaultAsync(c => c.Name == name);

        return city;
    }
}