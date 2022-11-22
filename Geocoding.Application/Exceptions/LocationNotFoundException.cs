using Base.Application.Exceptions;

namespace Geocoding.Application.Exceptions;

public class LocationNotFoundException : NotFoundException
{
    public LocationNotFoundException(string type)
        : base(type)
    {
    }
}