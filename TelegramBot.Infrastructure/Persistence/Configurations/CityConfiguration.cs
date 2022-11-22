using Bank.Domain.Entities;
using Base.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TelegramBot.Infrastructure.Persistence.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.HasOne(c => c.Location)
            .WithOne()
            .HasForeignKey<Location>(l => l.CityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}