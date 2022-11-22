using Base.Application.Exceptions;

namespace Geocoding.Application.Exceptions;

public class JsonNotFoundItemException : NotFoundException
{
    public JsonNotFoundItemException(string path)
        : base($"Not found '{path}'")
    {
    }
}