using Bank.Application.Interfaces;
using Bank.Domain.Entities;
using Bank.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bank.Infrastructure.Services;

public class UpdateExchangeInformation : IUpdateExchangeInformation
{
    private readonly IBankUpdater _bankUpdater;
    private readonly IBankContext _db;
    private readonly IGetHtmlDocument _getterDocument;
    private readonly ILogger _logger;
    private readonly int _updateTimeInSecond;


    public UpdateExchangeInformation(
        IBankContext db,
        IBankUpdater bankUpdater,
        IGetHtmlDocument getterDocument,
        int updateTimeInSecond,
        ILogger logger)
    {
        _db = db;
        _bankUpdater = bankUpdater;
        _getterDocument = getterDocument;
        _updateTimeInSecond = updateTimeInSecond;
        _logger = logger;
    }

    public async Task UpdateAsync(string cityName, bool checkTime = true)
    {
        var city = await _db.Cities.FirstOrDefaultAsync(c => c.Name.ToUpper() == cityName.ToUpper());
        if (city is null)
        {
            cityName = cityName.Capitalize();
            city = new City { Name = cityName };
            _logger.LogInformation("Add city '{CityName}'", cityName);
            // Todo: maybe exception
            _db.Cities.Add(city);
        }

        if (checkTime)
        {
            var diffTime = DateTime.Now.TickInSecond() - city.LastUpdate.TickInSecond();
            if (diffTime < _updateTimeInSecond)
            {
                _logger.LogTrace("Stopped update bank information");
                return;
            }
        }

        _getterDocument.Object = city.Name;

        await _bankUpdater.UpdateAsync(_getterDocument.Document, city);
        await _db.SaveChangesAsync(CancellationToken.None);
    }
}