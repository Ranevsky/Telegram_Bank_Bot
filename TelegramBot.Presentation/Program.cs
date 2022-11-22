using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace TelegramBot.Presentation;

public class Program
{
    public static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var builder = new HostBuilder();

        builder.ConfigureAppConfiguration(cfg =>
        {
            cfg.AddJsonFile("appsettings.json", false);
            cfg.AddUserSecrets(Assembly.GetExecutingAssembly(), false, true);
            cfg.AddEnvironmentVariables();
        });
        builder.UseSerilog();

        builder.ConfigureServices((context, services) => { services.AddTelegramBotPresentationServices(context); });

        var host = builder.Build();

        await host.StartAsync();

        var botStarter = host.Services.GetRequiredService<BotStarter>();

        await botStarter.StartBotAsync();
    }
}