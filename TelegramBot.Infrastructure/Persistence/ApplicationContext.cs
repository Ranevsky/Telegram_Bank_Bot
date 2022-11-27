using Bank.Application.Interfaces;
using Bank.Domain.Entities;
using Bank.Infrastructure.Persistence.Configurations;
using Base.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TelegramBot.Application.Interfaces;
using TelegramBot.Domain.Entities;
using TelegramBot.Infrastructure.Persistence.Configurations;
using CurrencyConfiguration = TelegramBot.Infrastructure.Persistence.Configurations.CurrencyConfiguration;

namespace TelegramBot.Infrastructure.Persistence;

public class ApplicationContext : DbContext, ITelegramContext, IBankContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
    }

    // Bank
    public DbSet<Bank.Domain.Entities.Bank> Banks => Set<Bank.Domain.Entities.Bank>();
    public DbSet<InternetBank> InternetBanks => Set<InternetBank>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<CurrencyExchange> CurrencyExchange => Set<CurrencyExchange>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<City> Cities => Set<City>();

    // Telegram
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Telegram
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new CurrencyConfiguration());

        // Bank
        modelBuilder.ApplyConfiguration(new BankConfiguration());
        modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
        modelBuilder.ApplyConfiguration(new LocationConfiguration());
        modelBuilder.ApplyConfiguration(new Bank.Infrastructure.Persistence.Configurations.CurrencyConfiguration());
        modelBuilder.ApplyConfiguration(new CurrencyExchangeConfiguration());
    }
}