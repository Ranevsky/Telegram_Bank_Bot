namespace Geocoding.Application.Exceptions;

public class ResultException : Exception
{
    public ResultException(string message)
        : base(message)
    {
    }
}