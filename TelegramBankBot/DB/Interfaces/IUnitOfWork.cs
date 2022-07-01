namespace TelegramBankBot.DB.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    Task SaveAsync();
}