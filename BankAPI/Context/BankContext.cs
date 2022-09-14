using BankAPI.Models;
using BankAPI.Models.Currencies;

using Microsoft.EntityFrameworkCore;

namespace BankAPI.Context;

public class BankContext : DbContext
{
    public DbSet<Bank> Banks => Set<Bank>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<City> Cities => Set<City>();

    public BankContext(DbContextOptions<BankContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Bank>()
            .HasMany(b => b.Departments)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Bank>()
            .HasMany(b => b.BestCurrencies)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Department>()
            .HasMany(d => d.Currencies)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

    }
}