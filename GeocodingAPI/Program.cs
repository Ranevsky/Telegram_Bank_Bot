using GeocodingAPI.Models;

using Logger;

namespace GeocodingAPI;

internal class Program
{
    private static async Task Main()
    {
        string key = "";
        string address = "";
        ILogger logger = new Logger.Models.EmptyLogger();

        GoogleGeocodingApi api = new(key, null, logger);

        await api.GetLocationAsync(address);
    }
}