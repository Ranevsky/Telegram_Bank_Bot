using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TelegramBot.Domain.Entities;

namespace TelegramBot.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.Id).ValueGeneratedNever();

        builder.HasOne(u => u.Location)
            .WithOne()
            .HasForeignKey<User>()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.NearCity)
            .WithMany();

        builder
            .HasOne(u => u.SelectedCurrency)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);
    }
}