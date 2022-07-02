namespace TelegramBankBot.DB.Interfaces;

public interface IRepository<T> where T : class
{
    public Task AddAsync(T entity);

    public Task<T?> FindAsync(T entity);
    public Task<T?> FindNoTrackingAsync(T entity);
    public Task<T?> FindAsync(long id);
    public Task<T?> FindNoTrackingAsync(long id);

    public void Remove(T entity);
    public Task RemoveProtectedAsync(T entity);

    public Task<bool> IsExistAsync(T entity);
    public Task<bool> IsExistAsync(long id);


    public IQueryable<T> GetAll();
}