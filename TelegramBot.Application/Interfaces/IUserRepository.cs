using Bank.Domain.Entities;
using TelegramBot.Application.Exceptions;
using TelegramBot.Domain.Entities;

namespace TelegramBot.Application.Interfaces;

public interface IUserRepository
{
    Task AddAsync(User user);

    /// <returns>if the user is added then true otherwise false</returns>
    Task<bool> AddWithCheckExistsAsync(User user);

    /// <exception cref="UserNotFoundException"></exception>
    Task DeleteAsync(long id);

    /// <exception cref="UserNotFoundException"></exception>
    void Delete(User? user);

    /// <exception cref="UserNotFoundException"></exception>
    Task<User> GetAsync(long id, bool tracking = false);

    /// <exception cref="UserNotFoundException"></exception>
    Task<User> GetWithLocationAsync(long id, bool tracking = false);

    /// <exception cref="UserNotFoundException"></exception>
    Task<User> GetWithCurrencyAsync(long id, bool tracking = false);

    /// <exception cref="UserNotFoundException"></exception>
    Task<User> GetWithLocationAndCurrencyAsync(long id, bool tracking = false);

    Task SetCityAsync(User user, City city);
    Task<User> GetWithLocationAndCurrencyAndNearCityAsync(long id, bool tracking = false);
    Task SetActiveAsync(long id, bool active);
    Task<bool> GetActiveAsync(long id);
    Task<bool?> GetBuyOperationAsync(long id);
}