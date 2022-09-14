namespace GeocodingAPI.Models.Interfaces;

public interface IGetLocationAndRadiusAsync
{
    /// <exception cref="JsonNotFoundItemException"></exception>
    /// <exception cref="ZeroResultException"></exception>
    /// <exception cref="ResultException"></exception>
    Task<LocationAndRadius> GetLocationAndRadiusAsync(string address);
}
