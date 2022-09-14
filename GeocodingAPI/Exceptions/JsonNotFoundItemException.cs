namespace GeocodingAPI.Exceptions;

public class JsonNotFoundItemException : Exception
{
    public JsonNotFoundItemException(string path)
        : base($"Not found '{path}'")
    {

    }
}
