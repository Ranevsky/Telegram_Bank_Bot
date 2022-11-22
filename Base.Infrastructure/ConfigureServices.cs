using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Base.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddDbContext<TContextService, TContextImplementation>(
        this IServiceCollection services,
        string connection)
        where TContextService : class
        where TContextImplementation : DbContext, TContextService
    {
        services.AddSqlServer<TContextImplementation>(connection);

        services.AddScoped<TContextService, TContextImplementation>();

        return services;
    }
}