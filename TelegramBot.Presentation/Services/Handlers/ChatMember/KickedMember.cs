using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Application.Exceptions;
using TelegramBot.Application.Interfaces;

namespace TelegramBot.Presentation.Services.Handlers.ChatMember;

public class KickedMember : Handler<ChatMemberUpdated>
{
    private readonly ILogger<KickedMember> _logger;
    private readonly IUnitOfWork _uow;

    public KickedMember(IUnitOfWork uow, ILogger<KickedMember> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public override async Task HandleAsync(ChatMemberUpdated memberUpdate)
    {
        var status = memberUpdate.NewChatMember.Status;

        if (status == ChatMemberStatus.Kicked)
        {
            var user = memberUpdate.From;
            try
            {
                await _uow.Users.SetActiveAsync(user.Id, false);
            }
            catch (UserNotFoundException)
            {
                _logger.LogWarning(user, "not found, but he stopped the bot");
            }
        }
        else
        {
            await Successor.HandleAsync(memberUpdate);
        }
    }
}