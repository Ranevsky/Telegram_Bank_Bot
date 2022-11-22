using Bank.Application.Interfaces;
using Bank.Domain.Entities;
using Bank.Infrastructure.Persistence.Configurations;
using Base.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bank.Infrastructure.Persistence;

public class BankContext : DbContext, IBankContext
{
    public BankContext(DbContextOptions<BankContext> options)
        : base(options)
    {
    }

    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Domain.Entities.Bank> Banks => Set<Domain.Entities.Bank>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<City> Cities => Set<City>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new BankConfiguration());
        modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
        modelBuilder.ApplyConfiguration(new LocationConfiguration());
        modelBuilder.ApplyConfiguration(new CurrencyConfiguration());
    }
}