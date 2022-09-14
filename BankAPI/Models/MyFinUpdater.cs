using Logger;

namespace BankAPI.Models;

public class MyFinUpdater : Updater
{
    public MyFinUpdater(ILogger logger)
        : base(new MyFinParser(), new MyFinChecker(logger))
    {

    }
}
