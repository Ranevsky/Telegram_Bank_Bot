using Bank.Domain.Entities;
using Base.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TelegramBot.Domain.Entities;

namespace TelegramBot.Application.Interfaces;

public interface ITelegramContext : IDisposable
{
    ChangeTracker ChangeTracker { get; }

    DbSet<User> Users { get; }
    DbSet<Location> Locations { get; }
    DbSet<Currency> Currencies { get; }
    DbSet<City> Cities { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
}