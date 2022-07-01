using Microsoft.EntityFrameworkCore;

using TelegramBankBot.DB.Interfaces;
using TelegramBankBot.Model;

namespace TelegramBankBot.DB;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationContext db) : base(db)
    {

    }

    public override async Task AddAsync(User user)
    {
        if (await IsExistAsync(user))
        {
#warning delete log
            Program.Log.Warning("User is exist");
            //throw new Exception("User is exists");
            return;
        }

        await Db.Set<User>().AddAsync(user);
    }

    public override async Task<User?> FindAsync(User entity)
    {
        return await Db.Users.FirstOrDefaultAsync(u => u.Id == entity.Id);
    }
    public override async Task<User?> FindNoTrackingAsync(User entity)
    {
        return await Db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == entity.Id);
    }

}
