namespace TelegramBankBot.DB.Interfaces;

public interface IRepository<T> where T : class
{
    public Task AddAsync(T entity);
    public Task<T?> FindAsync(T entity);
    public Task<T?> FindNoTrackingAsync(T entity);
    public void Remove(T entity);
    public Task RemoveProtectedAsync(T entity);
    public Task<bool> IsExistAsync(T entity);
    public IQueryable<T> GetAll();
}