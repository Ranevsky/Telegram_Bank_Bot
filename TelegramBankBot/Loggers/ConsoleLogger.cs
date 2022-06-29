using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBankBot;

public class ConsoleLogger : ILogger
{
    public ConsoleLogger()
    {
        Console.WriteLine("create");
    }
    public void Info(string message)
    {
        ColorText("INFO:", message, ConsoleColor.Green);
    }
    public void Warning(string message)
    {
        ColorText("WARNING:", message, ConsoleColor.Yellow);
    }
    public void Error(string message)
    {
        ColorText("ERROR:", message, ConsoleColor.Red);
    }

    private static void ColorText(string colorMessage, string message, ConsoleColor color)
    {
        var beginColor = Console.BackgroundColor;
        Console.BackgroundColor = color;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.Write(colorMessage);
        Console.BackgroundColor = beginColor;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($" {message}\n");
    }
}
