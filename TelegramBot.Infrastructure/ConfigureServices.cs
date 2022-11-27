using Bank.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using TelegramBot.Application.Interfaces;
using TelegramBot.Infrastructure.Persistence;
using TelegramBot.Infrastructure.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddTelegramBotInfrastructureServices(
        this IServiceCollection services,
        HostBuilderContext builderContext)
    {
        services.AddTelegramBotApplicationServices();
        services.AddApplicationContext(builderContext);
        services.AddTelegramRepositories();

        return services;
    }

    private static IServiceCollection AddApplicationContext(
        this IServiceCollection services,
        HostBuilderContext context)
    {
        var applicationContext = context.Configuration.GetConnectionString("ApplicationConnection");

        services.AddSqlServer<ApplicationContext>(applicationContext);

        services.AddScoped<IBankContext, ApplicationContext>();
        services.AddScoped<ITelegramContext, ApplicationContext>();

        return services;
    }

    private static IServiceCollection AddTelegramRepositories(this IServiceCollection services)
    {
        services.AddUserRepository();
        services.AddTelegramCurrencyRepository();
        services.AddUnitOfWork();

        return services;
    }

    private static IServiceCollection AddUserRepository(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    private static IServiceCollection AddTelegramCurrencyRepository(this IServiceCollection services)
    {
        services.AddScoped<ITelegramCurrencyRepository, TelegramCurrencyRepository>();

        return services;
    }

    private static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}