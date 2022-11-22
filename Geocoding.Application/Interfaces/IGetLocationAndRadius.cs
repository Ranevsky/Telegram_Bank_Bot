using Base.Domain.Models;
using Geocoding.Application.Exceptions;

namespace Geocoding.Application.Interfaces;

public interface IGetLocationAndRadiusAsync
{
    /// <exception cref="JsonNotFoundItemException"></exception>
    /// <exception cref="ZeroResultException"></exception>
    /// <exception cref="ResultException"></exception>
    Task<LocationAndRadius> GetLocationAndRadiusAsync(string address);
}