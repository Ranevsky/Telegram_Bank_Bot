using Bank.Domain.Entities;

namespace Bank.Application.Interfaces;

public interface ICityRepository
{
    Task<City> CreateIfNotExistAsync(string name);
}