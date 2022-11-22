using TelegramBot.Presentation.Models.Args;

namespace TelegramBot.Presentation.Services.Handlers.Text;

/// <summary>
///     Warning: Must be at the end of the search queue
/// </summary>
public class TextHandler : Handler<TextArgs>
{
    public override Task HandleAsync(TextArgs commandArgs)
    {
        return Task.CompletedTask;
    }
}