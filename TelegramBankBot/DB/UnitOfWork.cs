using TelegramBankBot.DB.Interfaces;

namespace TelegramBankBot.DB;

internal class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationContext db;
    private IUserRepository userRepository = null!;

    public UnitOfWork(ApplicationContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public IUserRepository Users
    {
        get
        {
            if (userRepository == null)
            {
                userRepository = new UserRepository(db);
            }
            return userRepository;
        }
    }
    public async Task SaveAsync()
    {
        await db.SaveChangesAsync();
#warning delete log
        Program.Log.Info("Save");
    }

    private bool disposed = false;
    private void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                db.Dispose();
            }
            disposed = true;
        }
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
