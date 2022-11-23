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

        builder.HasMany<User>()
            .WithOne(u => u.SelectedCurrency)
            .OnDelete(DeleteBehavior.SetNull);
    }
}