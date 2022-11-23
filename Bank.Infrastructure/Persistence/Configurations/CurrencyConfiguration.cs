using Bank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.Infrastructure.Persistence.Configurations;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.HasMany<CurrencyExchange>().WithOne(e => e.Currency).OnDelete(DeleteBehavior.Cascade);
    }
}