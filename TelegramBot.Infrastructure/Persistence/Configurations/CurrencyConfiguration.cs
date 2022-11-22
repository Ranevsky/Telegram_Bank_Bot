using Bank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TelegramBot.Domain.Entities;

namespace TelegramBot.Infrastructure.Persistence.Configurations;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.Property(u => u.Id).ValueGeneratedOnAdd();

        // 123.12345
        const int scale = 5;
        const int precision = 8;
        builder.Property(c => c.Buy).HasPrecision(precision, scale);
        builder.Property(c => c.Sell).HasPrecision(precision, scale);

        builder.HasMany<User>()
            .WithOne(u => u.SelectedCurrency)
            .OnDelete(DeleteBehavior.SetNull);
    }
}