using System.Text.Json.Nodes;

using GeocodingAPI.Exceptions;
using GeocodingAPI.Models.Interfaces;

using Logger;

namespace GeocodingAPI.Models;

public class GoogleGeocodingApi : IGeocodingAsync
{
    private const string URL = "https://maps.googleapis.com/maps/api/geocode/json";

    private readonly string _key;
    private readonly string? _language;
    private readonly ILogger _logger;

    public GoogleGeocodingApi(string key, string? language, ILogger logger)
    {
        _key = key;
        _language = language;
        _logger = logger;
    }

    /// <exception cref="JsonNotFoundItemException"></exception>
    /// <exception cref="ZeroResultException"></exception>
    /// <exception cref="ResultException"></exception>
    public async Task<Location> GetLocationAsync(string address)
    {
        JsonNode json = await GetAsync(address);
        Location location = HandleLocation(json);

        return location;
    }

    /// <exception cref="JsonNotFoundItemException"></exception>
    /// <exception cref="ZeroResultException"></exception>
    /// <exception cref="ResultException"></exception>
    public async Task<LocationAndRadius> GetLocationAndRadiusAsync(string address)
    {
        JsonNode json = await GetAsync(address);

        Location location = HandleLocation(json);
        double radius = HandleRadius(json);

        LocationAndRadius model = new()
        {
            Location = location,
            Radius = radius
        };

        return model;
    }

    /// <exception cref="JsonNotFoundItemException"></exception>
    /// <exception cref="ZeroResultException"></exception>
    /// <exception cref="ResultException"></exception>
    private async Task<JsonNode> GetAsync(string address)
    {
        string query = $"?address={address}{(_language == null ? "" : $"&lang={_language}")}&key={_key}";

        using HttpClient client = new();

        client.BaseAddress = new Uri(URL);
        HttpResponseMessage response = await client.GetAsync(query);
        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();
        JsonNode json = JsonNode.Parse(content)!;

        string status = json["status"]?.GetValue<string>()
            ?? throw new JsonNotFoundItemException("status");

        _logger.Info($"Used google api (geocoding) with address = '{address}'");
        switch (status)
        {
            case "OK":
            {
                break;
            }
            case "ZERO_RESULTS":
            {
                throw new ZeroResultException($"Address = '{address}' is not found");
            }
            default:
            {
                throw new ResultException($"An error has occurred. Status = '{status}'. Address = '{address}'");
            }
        }

        return json;
    }

    /// <exception cref="LocationNotFoundException"></exception>
    private static Location GetLocation(JsonNode json)
    {
        double lat = json["lat"]?
            .GetValue<double>()
            ?? throw new LocationNotFoundException(json.GetPath() + ":lat");

        double lng = json["lng"]?
            .GetValue<double>()
            ?? throw new LocationNotFoundException(json.GetPath() + ":lng");

        Location location = new()
        {
            Latitude = lat,
            Longitude = lng
        };

        return location;
    }

    /// <exception cref="JsonNotFoundItemException"></exception>
    private static Location HandleLocation(JsonNode json)
    {
        JsonNode locationJson = json["results"]?[0]?["geometry"]?["location"]
            ?? throw new JsonNotFoundItemException("results:[0]:geometry:location");

        Location location;
        try
        {
            location = GetLocation(locationJson);
        }
        catch (LocationNotFoundException ex)
        {
            throw new JsonNotFoundItemException(ex.Message);
        }

        return location;
    }

    /// <exception cref="JsonNotFoundItemException"></exception>
    private static double HandleRadius(JsonNode json)
    {
        JsonNode geometryJson = json["results"]?[0]?["geometry"]
            ?? throw new JsonNotFoundItemException("results:[0]:geometry");

        JsonNode regionJson = geometryJson["bounds"]
            ?? json["viewport"]
            ?? throw new JsonNotFoundItemException(geometryJson.GetPath() + $":bounds OR viewport");

        JsonNode northeastJson = regionJson["northeast"]
            ?? throw new JsonNotFoundItemException(regionJson.GetPath() + $":northeast");

        JsonNode southwestJson = regionJson["southwest"]
            ?? throw new JsonNotFoundItemException(regionJson.GetPath() + $":southwest");

        Location northeast;
        Location southwest;
        try
        {
            northeast = GetLocation(northeastJson);
            southwest = GetLocation(southwestJson);
        }
        catch (LocationNotFoundException ex)
        {
            throw new JsonNotFoundItemException(ex.Message);
        }

        // r = d/2
        double radius = Diff(northeast, southwest) / 2;

        return radius;

        static double Diff(Location location1, Location location2)
        {
            double p = Math.PI / 180;
            double diff = 0.5 - (Math.Cos((location2.Latitude - location1.Latitude) * p) / 2) + (Math.Cos(location1.Latitude * p) * Math.Cos(location2.Latitude * p) * (1 - Math.Cos((location2.Longitude - location1.Longitude) * p)) / 2);
            return 12742 * Math.Asin(Math.Sqrt(diff));
        }
    }
}