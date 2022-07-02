using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

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
        await Bot.SendMessageAsync(
            text: "Reply keyboard removed",
            replyMarkup: new ReplyKeyboardRemove());

        var user = await Program.UOW.Users.FindAsync(Bot.Id);        
        
        if (user == null)
        {
            throw new Exception("User is not exist in db, but used location");
        }

        user.Location = Program.Mapper.Map<TelegramBankBot.Model.Location>(_location);
        await Program.UOW.SaveAsync();
        
        Console.WriteLine($"Latitude: {user.Location.Latitude}, Longitude: {user.Location.Longitude}");
    }
}