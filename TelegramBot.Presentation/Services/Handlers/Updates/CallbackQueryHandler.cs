using AutoMapper;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Application.Exceptions;
using TelegramBot.Application.Interfaces;
using TelegramBot.Presentation.Exceptions;
using TelegramBot.Presentation.Models.Args;

namespace TelegramBot.Presentation.Services.Handlers.Updates;

public class CallbackQueryHandler : Handler<Update>
{
    private readonly Handler<CallbackArgs> _handler;
    private readonly ILogger<CallbackQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _uow;

    public CallbackQueryHandler(
        Handler<CallbackArgs> handler,
        IUnitOfWork uow,
        IMapper mapper,
        ILogger<CallbackQueryHandler> logger)
    {
        _handler = handler;
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public override async Task HandleAsync(Update update)
    {
        var type = update.Type;

        if (type == UpdateType.CallbackQuery)
        {
            var callbackArgs = _mapper.Map<CallbackArgs>(update.CallbackQuery!);
            var user = callbackArgs.From;

            var data = update.CallbackQuery!.Data ?? "Empty";
            _logger.LogInformation(user, "press button with callback data = '{CallbackData}'", data);

            try
            {
                var isActive = await _uow.Users.GetActiveAsync(user.Id);
                if (!isActive)
                {
                    _logger.LogWarning(user, "not active, but he press callback button");
                    return;
                }

                await _handler.HandleAsync(callbackArgs);
            }
            catch (HandlerNotFoundException)
            {
                _logger.LogError("Callback with data = '{Data}' not found handler", data);
            }
            catch (UserNotFoundException ex)
            {
                ex.SetData(user, callbackArgs.ChatId);
                throw;
            }
        }
        else
        {
            await Successor.HandleAsync(update);
        }
    }
}