using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddTelegramBotApplicationServices(this IServiceCollection services)
    {
        var executeAssembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(executeAssembly);

        return services;
    }
}