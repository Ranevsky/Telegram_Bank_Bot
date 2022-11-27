using Bank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.Infrastructure.Persistence.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.HasOne(c => c.Location)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}