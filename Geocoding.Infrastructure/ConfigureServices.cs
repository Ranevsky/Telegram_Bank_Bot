using Geocoding.Application.Interfaces;
using Geocoding.Infrastructure.Models;
using Geocoding.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddGeocodingInfrastructureServices(this IServiceCollection services)
    {
        services.AddGoogleGeocoding();

        return services;
    }

    private static IServiceCollection AddGoogleGeocoding(this IServiceCollection services)
    {
        services.AddSingleton<IGeocodingAsync, GoogleGeocodingApi>(serviceProvider =>
        {
            var cfg = serviceProvider.GetRequiredService<IConfiguration>();

            var googleCfg = cfg.GetRequiredSection("Google").Get<GoogleConfiguration>();

            var logger = serviceProvider.GetRequiredService<ILogger<GoogleGeocodingApi>>();

            var geocoding = new GoogleGeocodingApi(googleCfg.Key, googleCfg.Language, logger);

            return geocoding;
        });

        return services;
    }
}