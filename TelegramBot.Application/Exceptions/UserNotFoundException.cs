using Base.Application.Exceptions;
using TelegramBot.Domain.Entities;

namespace TelegramBot.Application.Exceptions;

public class UserNotFoundException : NotFoundException
{
    public User? User { get; set; }
    public long ChatId { get; set; }

    public void SetData(User user, long chatId)
    {
        User = user;
        ChatId = chatId;
    }
}