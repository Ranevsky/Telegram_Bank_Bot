using BankAPI.Context;
using BankAPI.Extensions;
using BankAPI.Models;
using BankAPI.Models.HtmlDocuments.Interfaces;
using BankAPI.Models.Interfaces;
using BankAPI.Repositories.Interfaces;

using GeocodingAPI.Models;
using GeocodingAPI.Models.Interfaces;

using Logger;

using Microsoft.EntityFrameworkCore;

namespace BankAPI.Repositories;

public class BankRepository : IBankRepository
{
    private readonly int _updateTimeInSecond;
    private readonly BankContext _db;
    private readonly IGeocodingAsync _getLocation;
    private readonly IBankUpdater _bankUpdater;
    private readonly IGetHtmlDocument _getterDocument;
    private readonly ILogger _logger;

    public BankRepository(
        BankContext db,
        int updateTimeInSecond,
        IGeocodingAsync getLocation,
        IBankUpdater bankUpdater,
        IGetHtmlDocument getterDocument,
        ILogger logger)
    {
        _db = db;
        _updateTimeInSecond = updateTimeInSecond;
        _getLocation = getLocation;
        _bankUpdater = bankUpdater;
        _getterDocument = getterDocument;
        _logger = logger;
    }

    public async Task<List<Bank>> GetBanksAsync()
    {
        return await _db.Banks
            .Include(b => b.BestCurrencies)
            .ToListAsync();
    }
    public async Task<List<Bank>> GetBanksByCityAsync(string cityName, bool onUpdate = true)
    {
        if (onUpdate)
        {
            await UpdateAsync(cityName, true);
            //await _db.SaveChangesAsync();
        }

        cityName = cityName.ToUpper();
        return await _db.Banks
            .Include(b => b.BestCurrencies)
            .Include(b => b.Departments.Where(d => d.City.Name.ToUpper() == cityName))
                .ThenInclude(d => d.Currencies)
            .ToListAsync();
    }
    public async Task UpdateAsync(string cityName, bool checkTime = true)
    {
        City? city = await _db.Cities.FirstOrDefaultAsync(c => c.Name.ToUpper() == cityName.ToUpper());
        if (city == null)
        {
            cityName = cityName.Capitalize();
            city = new() { Name = cityName };
            _logger.Info($"Add city '{cityName}'");
            _db.Cities.Add(city);
        }

        long diffTime = DateTime.Now.TickInSecond() - city.LastUpdate.TickInSecond();
        if (checkTime && diffTime < _updateTimeInSecond)
        {
            _logger.Info("Update stoped");
            return;
        }

        _getterDocument.Object = city.Name;

        await _bankUpdater.UpdateAsync(_db, _getterDocument.Document, city);
    }
    public async Task CheckLocationAsync(string cityName)
    {
        City city = (await _db.Cities
            .Include(c => c.Location)
            .FirstOrDefaultAsync(c => c.Name.ToUpper() == cityName.ToUpper()))!;

        await CheckLocationAsync(city);
    }
    /// <exception cref="ArgumentNullException"></exception>
    public async Task CheckLocationAsync(City city)
    {
        if (city == null)
        {
            throw new ArgumentNullException(nameof(city));
        }

        IEnumerable<List<Department>> departmentsCollection = _db.Banks
            .Include(b => b.Departments.Where(d => d.City == city))
                .ThenInclude(d => d.Location)
            .Select(b => b.Departments)
            .ToList();

        bool isSave = true;
        if (city.Radius == null)
        {
            LocationAndRadius locationAndRadius = await _getLocation.GetLocationAndRadiusAsync(city.Name);
            city.Radius = locationAndRadius.Radius;
            if (city.Location == null)
            {
                city.Location = (BankAPI.Models.Location)locationAndRadius.Location;
            }
        }
        else if (city.Location == null)
        {
            city.Location = (BankAPI.Models.Location)await _getLocation.GetLocationAsync(city.Name);
        }
        else
        {
            isSave = false;
        }

        try
        {
            foreach (List<Department> departments in departmentsCollection)
            {
                foreach (Department department in departments)
                {
                    if (department.Location != null)
                    {
                        continue;
                    }

                    department.Location = (BankAPI.Models.Location)await _getLocation.GetLocationAsync(department.Street);

                    double distance = BankAPI.Models.Location.Diff(city.Location, department.Location);
                    if (distance >= city.Radius)
                    {
                        _logger.Error($"Department with id = '{department.Id}', not located in radius of occurrence = '{city.Radius}', distance = '{distance}'");
                        department.Location = null;
                    }
                    else
                    {
                        isSave = true;
                    }
                }
            }
        }
        finally
        {
            if (isSave)
            {
                await _db.SaveChangesAsync();
            }
        }
    }
}
