using AutoMapper;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Application.Interfaces;
using TelegramBot.Presentation.Exceptions;
using TelegramBot.Presentation.Services.Handlers.Commands.Remove;
using Location = Base.Domain.Entities.Location;

namespace TelegramBot.Presentation.Services.Handlers.Messages;

public class LocationHandler : Handler<Message>
{
    private readonly ITelegramBotClient _bot;
    private readonly ILogger<LocationHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _uow;

    public LocationHandler(ITelegramBotClient bot, IUnitOfWork uow, IMapper mapper, ILogger<LocationHandler> logger)
    {
        _bot = bot;
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public override async Task HandleAsync(Message message)
    {
        var type = message.Type;
        if (type == MessageType.Location)
        {
            var userTg = message.From!;

            _logger.LogInformation(userTg, "send location");

            var userDb = await _uow.Users.GetWithLocationAsync(userTg.Id, true);

            await RemoveKeyboard.RemoveKeyboardAsync(_bot, null, message.Chat.Id);

#warning look in location
            var location = _mapper.Map<Location>(message.Location!);
            if (userDb.Location is null)
            {
                userDb.Location = location;
            }
            else
            {
                userDb.Location.Latitude = location.Latitude;
                userDb.Location.Longitude = location.Longitude;
            }

            var nearCity = _uow.Banks
                .GetCities()
                .MinBy(c => Location.Distance(c.Location!, userDb.Location));

            if (nearCity is null)
            {
                throw new NearCityNotFoundException();
            }

            nearCity.Id = 0;
            if (nearCity.Location is not null)
            {
                nearCity.Location.Id = 0;
            }

            await _uow.Users.SetCityAsync(userDb, nearCity);

            await _uow.SaveAsync();

            _logger.LogInformation(userDb, "change location");
        }
        else
        {
            await Successor.HandleAsync(message);
        }
    }
}