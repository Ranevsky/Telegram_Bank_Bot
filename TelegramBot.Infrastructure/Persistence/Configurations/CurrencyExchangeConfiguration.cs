using Bank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TelegramBot.Infrastructure.Persistence.Configurations;

public class CurrencyExchangeConfiguration : IEntityTypeConfiguration<CurrencyExchange>
{
    public void Configure(EntityTypeBuilder<CurrencyExchange> builder)
    {
        builder.HasOne(c => c.Currency).WithMany();

        // 123.12345
        const int scale = 5;
        const int precision = 8;
        builder.Property(c => c.Buy).HasPrecision(precision, scale);
        builder.Property(c => c.Sell).HasPrecision(precision, scale);
    }
}