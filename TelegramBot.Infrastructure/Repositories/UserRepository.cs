using Bank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TelegramBot.Application.Exceptions;
using TelegramBot.Application.Interfaces;
using TelegramBot.Domain.Entities;

namespace TelegramBot.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ITelegramContext _db;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ITelegramContext db, ILogger<UserRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<bool> AddWithCheckExistsAsync(User user)
    {
        var userInDb = await GetBaseInclusion(true)
            .Where(u => u.Id == user.Id)
            .FirstOrDefaultAsync();

        if (userInDb is not null)
        {
            if (userInDb.IsActive == false)
            {
                userInDb.IsActive = true;
                await _db.SaveChangesAsync(CancellationToken.None);
            }

            return false;
        }

        await AddAsync(user);
        await _db.SaveChangesAsync(CancellationToken.None);
        return true;
    }

    public async Task AddAsync(User user)
    {
        await _db.Users.AddAsync(user);
        _logger.LogInformation(user, "added db");
    }

    public async Task DeleteAsync(long id)
    {
        var user = await GetAsync(id, true);

        Delete(user);
    }

    public void Delete(User? user)
    {
        if (user is null)
        {
            return;
        }

        _db.Users.Remove(user);
        _logger.LogInformation(user, "removed db");
    }

    public async Task<User> GetAsync(long id, bool tracking = false)
    {
        var user = await GetAsync(id, GetBaseInclusion(tracking));

        return user;
    }

    public async Task<User> GetWithLocationAsync(long id, bool tracking = false)
    {
        var user = await GetAsync(id, GetLocationInclusion(tracking));

        return user;
    }

    public async Task<User> GetWithLocationAndCurrencyAsync(long id, bool tracking = false)
    {
        var user = await GetAsync(id, GetLocationAndCurrencyInclusion(tracking));

        return user;
    }

    public async Task<User> GetWithLocationAndCurrencyAndNearCityAsync(long id, bool tracking = false)
    {
        var user = await GetAsync(id, GetLocationAndCurrencyAndNearCityInclusion(tracking));

        return user;
    }

    public async Task SetActiveAsync(long id, bool active)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id)
                   ?? throw new UserNotFoundException();

        user.IsActive = active;
        await _db.SaveChangesAsync(CancellationToken.None);
    }

    public async Task<bool> GetActiveAsync(long id)
    {
        var user = await _db.Users.Select(u => new { u.Id, u.IsActive })
                       .FirstOrDefaultAsync(u => u.Id == id)
                   ?? throw new UserNotFoundException();

        return user.IsActive;
    }

    public async Task<bool?> GetBuyOperationAsync(long id)
    {
        var model = await _db.Users.Select(u => new { u.Id, u.IsBuyOperation }).FirstOrDefaultAsync(u => u.Id == id)
                    ?? throw new UserNotFoundException();

        return model.IsBuyOperation;
    }

    public async Task<User> GetWithCurrencyAsync(long id, bool tracking = false)
    {
        var user = await GetAsync(id, GetCurrencyInclusion(tracking));

        return user;
    }

    public async Task SetCityAsync(User user, City city)
    {
        var cityDb = await _db.Cities.FirstOrDefaultAsync(c => c.Name == city.Name);

        if (cityDb is null)
        {
            await _db.Cities.AddAsync(city);
            cityDb = city;
        }

        user.NearCity = cityDb;
    }

    /// <exception cref="UserNotFoundException"></exception>
    private static async Task<User> GetAsync(long id, IQueryable<User> inclusions)
    {
        var user = await inclusions.FirstOrDefaultAsync(u => u.Id == id);

        return user ?? throw new UserNotFoundException();
    }

    private IQueryable<User> GetBaseInclusion(bool tracking = false)
    {
        IQueryable<User> query = _db.Users;

        return tracking
            ? query.AsTracking()
            : query.AsNoTracking();
    }

    private IQueryable<User> GetLocationInclusion(bool tracking = false)
    {
        return GetBaseInclusion(tracking)
            .Include(u => u.Location)
            .Include(u => u.NearCity)
            .ThenInclude(c => c!.Location);
    }

    private IQueryable<User> GetCurrencyInclusion(bool tracking = false)
    {
        return GetBaseInclusion(tracking)
            .Include(u => u.SelectedCurrency);
    }

    private IQueryable<User> GetLocationAndCurrencyInclusion(bool tracking = false)
    {
        return GetBaseInclusion(tracking)
            .Include(u => u.Location)
            .Include(u => u.SelectedCurrency);
    }

    private IQueryable<User> GetLocationAndCurrencyAndNearCityInclusion(bool tracking = false)
    {
        return GetBaseInclusion(tracking)
            .Include(u => u.Location)
            .Include(u => u.NearCity)
            .ThenInclude(c => c!.Location)
            .Include(u => u.SelectedCurrency);
    }
}