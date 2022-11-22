using Base.Application.Exceptions;

namespace TelegramBot.Presentation.Exceptions;

public class NearCityNotFoundException : NotFoundException
{
    public NearCityNotFoundException()
        : base("Near city is not found in db")
    {
    }

    public NearCityNotFoundException(string message)
        : base(message)
    {
    }
}