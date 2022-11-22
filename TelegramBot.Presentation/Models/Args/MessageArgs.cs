namespace TelegramBot.Presentation.Models.Args;

public class MessageArgs : MessageInfo
{
    public string[] Args { get; set; } = null!;
    public int ArgsIteration { get; set; } = 0;

    public string GetArg()
    {
        return Args[ArgsIteration];
    }
}