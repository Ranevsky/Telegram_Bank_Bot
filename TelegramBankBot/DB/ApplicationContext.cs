﻿using Microsoft.EntityFrameworkCore;

using TelegramBankBot.Model;

namespace TelegramBankBot.DB;

public class ApplicationContext : DbContext
{
    private static readonly string _connection = Program.Configuration.GetSection("ConnectionStrings")["DefaultConnection"];
    public DbSet<User> Users { get; set; } = null!;

    public ApplicationContext() : base()
    {
        Database.EnsureCreated();
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_connection);
        base.OnConfiguring(optionsBuilder);
    }
}