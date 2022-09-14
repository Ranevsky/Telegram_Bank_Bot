namespace Logger.Models;

public class ConsoleLogger : ILogger
{
    public ConsoleLogger()
    {

    }
    public void Info(string message)
    {
        ColorText($"INFO", $"[{DateTime.Now:G}] {message}", ConsoleColor.Green);
    }
    public void Warning(string message)
    {
        ColorText("WARNING", $"[{DateTime.Now:G}] {message}", ConsoleColor.Yellow);
    }
    public void Error(string message)
    {
        ColorText("ERROR", $"[{DateTime.Now:G}] {message}", ConsoleColor.Red);
    }

    private static void ColorText(string colorMessage, string message, ConsoleColor color)
    {
        ConsoleColor beginColor = Console.BackgroundColor;
        Console.BackgroundColor = color;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.Write(colorMessage);
        Console.BackgroundColor = beginColor;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"{message}{Environment.NewLine}");
    }
}
