using Bank.Application.Interfaces;
using Bank.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;

namespace Bank.Infrastructure.Services;

public class UpdateExchangeInformation : IUpdateExchangeInformation
{
    private readonly IBankUpdater _bankUpdater;
    private readonly ICityRepository _cityRepository;
    private readonly ILogger _logger;
    private readonly int _updateTimeInSecond;


    public UpdateExchangeInformation(
        ICityRepository cityRepository,
        IBankUpdater bankUpdater,
        int updateTimeInSecond,
        ILogger logger)
    {
        _cityRepository = cityRepository;
        _bankUpdater = bankUpdater;
        _updateTimeInSecond = updateTimeInSecond;
        _logger = logger;
    }

    public async Task UpdateAsync(string cityName, bool checkTime = true)
    {
        if (!checkTime)
        {
            return;
        }

        var city = await _cityRepository.CreateIfNotExistAsync(cityName);

        var diffTime = DateTime.Now.TickInSecond() - city.LastUpdate.TickInSecond();
        if (diffTime < _updateTimeInSecond)
        {
            _logger.LogTrace("Stopped update bank information");
            return;
        }

        await _bankUpdater.UpdateAsync(city);
    }
}