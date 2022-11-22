using Bank.Application.Interfaces;
using Bank.Infrastructure.Persistence;
using Bank.Infrastructure.Repositories;
using Bank.Infrastructure.Services;
using Bank.Infrastructure.Services.HtmlDocuments;
using Base.Infrastructure;
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
        services.AddBankContext(context);
        services.AddBankRepositories();

        return services;
    }

    private static IServiceCollection AddBankUpdater(this IServiceCollection services)
    {
        services.AddScoped<MyFinParser>();
        services.AddScoped<BankChecker>();

        services.AddScoped<IBankUpdater, MyFinBankUpdater>(serviceProvider =>
        {
            var parser = serviceProvider.GetRequiredService<MyFinParser>();
            var checker = serviceProvider.GetRequiredService<BankChecker>();

            var updater = new MyFinBankUpdater(parser, checker);

            return updater;
        });

        return services;
    }

    private static IServiceCollection AddGetHtmlDocument(this IServiceCollection services)
    {
        services.AddScoped<IGetHtmlDocument, GetterMyFinHtmlDocument>();

        return services;
    }

    private static IServiceCollection AddBankContext(this IServiceCollection services, HostBuilderContext context)
    {
        var bankConnection = context.Configuration.GetConnectionString("BankConnection");

        services.AddDbContext<IBankContext, BankContext>(bankConnection);

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
            var db = serviceProvider.GetRequiredService<IBankContext>();
            var bankUpdater = serviceProvider.GetRequiredService<IBankUpdater>();
            var getterHtml = serviceProvider.GetRequiredService<IGetHtmlDocument>();
            var logger = serviceProvider.GetRequiredService<ILogger<UpdateExchangeInformation>>();

            var updateExchange = new UpdateExchangeInformation(db, bankUpdater, getterHtml, updateTimeInSecond, logger);

            return updateExchange;
        });

        return services;
    }

    private static IServiceCollection AddBankRepositories(this IServiceCollection services)
    {
        services.AddBankRepository();
        services.AddDepartmentRepository();

        return services;
    }

    private static IServiceCollection AddBankRepository(this IServiceCollection services)
    {
        services.AddScoped<IBankRepository, BankRepository>();

        return services;
    }

    private static IServiceCollection AddDepartmentRepository(this IServiceCollection services)
    {
        services.AddSingleton<IDepartmentRepository, DepartmentRepository>();

        return services;
    }
}