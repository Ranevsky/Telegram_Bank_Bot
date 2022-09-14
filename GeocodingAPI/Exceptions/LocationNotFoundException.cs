namespace GeocodingAPI.Exceptions;

internal class LocationNotFoundException : Exception
{
    public LocationNotFoundException(string type)
        : base(type)
    {

    }
}
