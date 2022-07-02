using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using TelegramBankBot.Model;

namespace TelegramBankBot.DB;

public class ApplicationContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Location> Locations => Set<Location>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {  
        base.OnConfiguring(optionsBuilder);

        ConfigurationBuilder? builder = new ConfigurationBuilder();

        builder.AddJsonFile("appsettings.json");
        IConfigurationRoot? config = builder.Build();
        string connectionString = config.GetConnectionString("DefaultConnection");
        optionsBuilder.UseSqlite(connectionString);
    }
}