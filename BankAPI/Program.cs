using BankAPI.Context;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BankAPI;

internal class Program
{
    private static void Main()
    {
        //Host();
    }

    private static void Host()
    {
        HostBuilder hostBuilder = new();
        hostBuilder.ConfigureHostConfiguration(cfg =>
        {
            cfg.AddJsonFile("appsettings.json", false);
        });

        hostBuilder.ConfigureServices((context, services) =>
        {
            string connection = context.Configuration.GetConnectionString("Default");

            services.AddSqlite<BankContext>(connection);
        });

        hostBuilder.Build().Run();
    }
}