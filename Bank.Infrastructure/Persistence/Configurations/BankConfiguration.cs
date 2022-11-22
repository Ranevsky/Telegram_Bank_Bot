using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.Infrastructure.Persistence.Configurations;

public class BankConfiguration : IEntityTypeConfiguration<Domain.Entities.Bank>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Bank> builder)
    {
        builder.Property(b => b.Id).ValueGeneratedOnAdd();

        builder.HasMany(b => b.Departments)
            .WithOne(d => d.Bank)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(b => b.BestCurrencies)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}