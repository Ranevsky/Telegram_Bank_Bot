using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Telegram.Bot;
using TelegramBot.Presentation;
using TelegramBot.Presentation.Extensions;
using TelegramBot.Presentation.Profiles;
using TelegramBot.Presentation.Profiles.Args;
using TelegramBot.Presentation.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddTelegramBotPresentationServices(
        this IServiceCollection services,
        HostBuilderContext context)
    {
        services.AddSerilog(context.Configuration);
        services.AddTelegramBotInfrastructureServices(context);

        services.AddHandlers();
        services.AddBotStarter();
        services.AddTelegramBotClient();

        services.AddBankInfrastructureServices(context);

        services.AddGeocodingInfrastructureServices();
        services.AddProfiles();

        services.AddGetExchange();
        return services;
    }

    private static IServiceCollection AddProfiles(this IServiceCollection services)
    {
        Type[] types =
        {
            typeof(CallbackArgsProfile),
            typeof(CommandArgsProfile),
            typeof(TextArgsProfile),
            typeof(ChatProfile),
            typeof(LocationProfile),
            typeof(UserProfile)
        };

        services.AddAutoMapper(types);

        return services;
    }

    private static IServiceCollection AddSerilog(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        services.AddLogging(configure => { configure.AddSerilog(dispose: true); });

        return services;
    }

    private static IServiceCollection AddBotStarter(this IServiceCollection services)
    {
        services.AddSingleton<BotStarter>();

        return services;
    }

    private static IServiceCollection AddGetExchange(this IServiceCollection services)
    {
        services.AddScoped<GetExchange>();

        return services;
    }

    private static IServiceCollection AddTelegramBotClient(this IServiceCollection services)
    {
        services.AddSingleton<ITelegramBotClient, TelegramBotClient>(serviceProvider =>
        {
            var cfg = serviceProvider.GetRequiredService<IConfiguration>();

            var token = cfg.GetRequiredSection("Telegram:Token").Value;

            TelegramBotClient bot = new(token!);
            return bot;
        });
        return services;
    }
}