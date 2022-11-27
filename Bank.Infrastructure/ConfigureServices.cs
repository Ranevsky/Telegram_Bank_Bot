using Bank.Application.Interfaces;
using Bank.Infrastructure.Repositories;
using Bank.Infrastructure.Services;
using Bank.Infrastructure.Services.HtmlDocuments;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddBankInfrastructureServices(
        this IServiceCollection services,
        HostBuilderContext context)
    {
        services.AddBankUpdater();
        services.AddGetHtmlDocument();
        services.AddUpdateExchangeInformation(context);
        services.AddBankRepositories();

        return services;
    }

    private static IServiceCollection AddBankUpdater(this IServiceCollection services)
    {
        services.AddScoped<MyFinGetterHtmlDocument>();
        services.AddScoped<MyFinParser>();
        services.AddScoped<GetBanksFromMyFin>();
        services.AddScoped<IGetBanks, GetBanksFromMyFin>();
        services.AddScoped<IBankChecker, BankChecker>();
        services.AddScoped<IBankUpdater, MyFinBankUpdater>();

        return services;
    }

    private static IServiceCollection AddGetHtmlDocument(this IServiceCollection services)
    {
        services.AddScoped<IGetHtmlDocument, MyFinGetterHtmlDocument>();

        return services;
    }

    private static IServiceCollection AddUpdateExchangeInformation(
        this IServiceCollection services,
        HostBuilderContext context)
    {
        var updateTimeInSecond =
            int.Parse(context.Configuration.GetRequiredSection("BankApi:UpdateTimeInSecond").Value);

        services.AddScoped<IUpdateExchangeInformation, UpdateExchangeInformation>(serviceProvider =>
        {
            var cityRepository = serviceProvider.GetRequiredService<ICityRepository>();
            var bankUpdater = serviceProvider.GetRequiredService<IBankUpdater>();
            var logger = serviceProvider.GetRequiredService<ILogger<UpdateExchangeInformation>>();

            var updateExchange = new UpdateExchangeInformation(cityRepository, bankUpdater, updateTimeInSecond, logger);

            return updateExchange;
        });

        return services;
    }

    private static IServiceCollection AddBankRepositories(this IServiceCollection services)
    {
        services.AddBankRepository();
        services.AddDepartmentRepository();
        services.AddCityRepository();

        return services;
    }

    private static IServiceCollection AddBankRepository(this IServiceCollection services)
    {
        services.AddScoped<IBankRepository, BankRepository>();

        return services;
    }

    private static IServiceCollection AddDepartmentRepository(this IServiceCollection services)
    {
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();

        return services;
    }

    private static IServiceCollection AddCityRepository(this IServiceCollection services)
    {
        services.AddScoped<ICityRepository, CityRepository>();

        return services;
    }
}