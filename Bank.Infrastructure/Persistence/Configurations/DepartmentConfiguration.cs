using Bank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.Infrastructure.Persistence.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.Property(d => d.Id).ValueGeneratedOnAdd();

        builder.HasMany(d => d.Currencies)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}