using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBankBot.Handlers;

public static class LocationHandler
{
    public static async Task HandleAsync(Bot bot, Location location)
    {
        await bot.SendMessageAsync(
            text: "Reply keyboard removed",
            replyMarkup: new ReplyKeyboardRemove());

        Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}");
    }
}