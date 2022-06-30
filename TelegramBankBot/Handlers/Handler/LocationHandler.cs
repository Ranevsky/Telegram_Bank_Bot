using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;

namespace TelegramBankBot.Handlers;

public class LocationHandler : Handler
{
    private readonly Location _location = null!;
    public LocationHandler(Bot bot, Location location) : base(bot)
    {
        _location = location;
    }
    public override async Task HandleAsync()
    {
        var msg = await Bot.SendMessageAsync(
            text: "Reply keyboard removed",
            replyMarkup: new ReplyKeyboardRemove());

        Console.WriteLine($"Latitude: {_location.Latitude}, Longitude: {_location.Longitude}");
    }
}