using System.Text.Json.Nodes;
using Base.Domain.Entities;
using Base.Domain.Models;
using Geocoding.Application.Exceptions;
using Geocoding.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Geocoding.Infrastructure.Services;

public class GoogleGeocodingApi : IGeocodingAsync
{
    private const string Url = "https://maps.googleapis.com/maps/api/geocode/json";

    private readonly string _key;
    private readonly string? _language;
    private readonly ILogger<GoogleGeocodingApi> _logger;

    public GoogleGeocodingApi(string key, string? language, ILogger<GoogleGeocodingApi> logger)
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
        var json = await GetAsync(address);
        var location = HandleLocation(json);

        return location;
    }

    /// <exception cref="JsonNotFoundItemException"></exception>
    /// <exception cref="ZeroResultException"></exception>
    /// <exception cref="ResultException"></exception>
    public async Task<LocationAndRadius> GetLocationAndRadiusAsync(string address)
    {
        var json = await GetAsync(address);

        var location = HandleLocation(json);
        var radius = HandleRadius(json);

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
        var query = $"?address={address}{(_language is null ? "" : $"&lang={_language}")}&key={_key}";

        using HttpClient client = new();

        client.BaseAddress = new Uri(Url);
        var response = await client.GetAsync(query);

        _logger.LogInformation("Used {ProviderService} api ({Service}) with address = '{Address}'", "google",
            "geocoding", address);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var json = JsonNode.Parse(content)!;

        var status = json["status"]?.GetValue<string>()
                     ?? throw new JsonNotFoundItemException("status");

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
        var lat = json["lat"]?
                      .GetValue<double>()
                  ?? throw new LocationNotFoundException(json.GetPath() + ":lat");

        var lng = json["lng"]?
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
        var locationJson = json["results"]?[0]?["geometry"]?["location"]
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
        var geometryJson = json["results"]?[0]?["geometry"]
                           ?? throw new JsonNotFoundItemException("results:[0]:geometry");

        var regionJson = geometryJson["bounds"]
                         ?? json["viewport"]
                         ?? throw new JsonNotFoundItemException(geometryJson.GetPath() + ":bounds OR viewport");

        var northeastJson = regionJson["northeast"]
                            ?? throw new JsonNotFoundItemException(regionJson.GetPath() + ":northeast");

        var southwestJson = regionJson["southwest"]
                            ?? throw new JsonNotFoundItemException(regionJson.GetPath() + ":southwest");

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
        var radius = Location.Distance(northeast, southwest) / 2;

        return radius;
    }
}