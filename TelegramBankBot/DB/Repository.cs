using Microsoft.EntityFrameworkCore;

using TelegramBankBot.DB.Interfaces;
using TelegramBankBot.Model.Interfaces;

namespace TelegramBankBot.DB;

public class Repository<T> : IRepository<T> where T : class, IEntityId
{
    protected readonly ApplicationContext Db;
    public Repository(ApplicationContext db)
    {
        Db = db;
    }
    public virtual async Task AddAsync(T entity)
    {
        await Db.Set<T>().AddAsync(entity);
    }
    public void Remove(T entity)
    {
        Db.Set<T>().Remove(entity);
    }
    public async Task RemoveProtectedAsync(T entity)
    {
        if (!await IsExistAsync(entity))
        {
#warning delete log
            Program.Log.Error($"Delete '{nameof(entity)}' but is not exist");
            return;
        }
        Remove(entity);
    }
    public async Task<bool> IsExistAsync(long id)
    {
        T? existEntity = await FindNoTrackingAsync(id);
        return existEntity != null;
    }
    public async Task<bool> IsExistAsync(T entity)
    {
        T? existEntity = await FindNoTrackingAsync(entity);
        return existEntity != null;
    }
    public virtual async Task<T?> FindAsync(T entity)
    {
        return await Db.Set<T>().FindAsync(entity);
    }
    public virtual async Task<T?> FindNoTrackingAsync(T entity)
    {
        return await Db.Set<T>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == entity.Id);
    }
    public IQueryable<T> GetAll()
    {
        return Db.Set<T>().AsQueryable();
    }

    public virtual async Task<T?> FindAsync(long id)
    {
        return await Db.Set<T>().FirstOrDefaultAsync(entity => entity.Id == id);
    }


    public virtual async Task<T?> FindNoTrackingAsync(long id)
    {
        return await Db.Set<T>().AsNoTracking().FirstOrDefaultAsync(entity => entity.Id == id);
    }
}
