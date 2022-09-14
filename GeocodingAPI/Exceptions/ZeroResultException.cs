namespace GeocodingAPI.Exceptions;

public class ZeroResultException : ResultException
{
    public ZeroResultException(string message) : base(message)
    {

    }
}