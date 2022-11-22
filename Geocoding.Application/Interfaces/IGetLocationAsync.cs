using Base.Domain.Entities;
using Geocoding.Application.Exceptions;

namespace Geocoding.Application.Interfaces;

public interface IGetLocationAsync
{
    /// <exception cref="JsonNotFoundItemException"></exception>
    /// <exception cref="ZeroResultException"></exception>
    /// <exception cref="ResultException"></exception>
    Task<Location> GetLocationAsync(string address);
}