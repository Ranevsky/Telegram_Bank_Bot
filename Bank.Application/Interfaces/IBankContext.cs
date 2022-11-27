using Bank.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bank.Application.Interfaces;

public interface IBankContext : IDisposable
{
    DbSet<Domain.Entities.Bank> Banks { get; }
    DbSet<InternetBank> InternetBanks { get; }
    DbSet<Department> Departments { get; }
    DbSet<Currency> Currencies { get; }
    DbSet<CurrencyExchange> CurrencyExchange { get; }
    DbSet<City> Cities { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}