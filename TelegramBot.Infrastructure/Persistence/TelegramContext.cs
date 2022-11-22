using Bank.Domain.Entities;
using Base.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TelegramBot.Application.Interfaces;
using TelegramBot.Domain.Entities;
using TelegramBot.Infrastructure.Persistence.Configurations;

namespace TelegramBot.Infrastructure.Persistence;

public class TelegramContext : DbContext, ITelegramContext
{
    public TelegramContext(DbContextOptions<TelegramContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<City> Cities => Set<City>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new CurrencyConfiguration());
    }
}