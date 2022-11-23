using Base.Infrastructure;
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
        services.AddTelegramContext(builderContext);
        services.AddTelegramRepositories();

        return services;
    }

    private static IServiceCollection AddTelegramContext(this IServiceCollection services, HostBuilderContext context)
    {
        var telegramConnection = context.Configuration.GetConnectionString("TelegramConnection");

        services.AddDbContext<ITelegramContext, TelegramContext>(telegramConnection);

        return services;
    }

    private static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

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
}