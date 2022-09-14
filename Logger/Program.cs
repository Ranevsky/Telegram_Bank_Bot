using Logger.Models;

namespace Logger;

internal class Program
{
    private static void Main()
    {
        ILogger logger = new ConsoleLogger();
        Test(logger);
    }
    private static void Test(ILogger logger)
    {
        logger.Info("Test info");
        logger.Warning("Test warning");
        logger.Error("Test error");
    }
}