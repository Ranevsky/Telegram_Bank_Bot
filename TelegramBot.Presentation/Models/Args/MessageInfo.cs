using TelegramBot.Domain.Entities;

namespace TelegramBot.Presentation.Models.Args;

public class MessageInfo
{
    public User From { get; set; } = null!;
    public long ChatId { get; set; }
    public int MessageId { get; set; }
}