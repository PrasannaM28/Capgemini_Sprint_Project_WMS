using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Persistence;

namespace WMS.Infrastructure.Repositories;

public class UserLoginRepository
    : GenericRepository<UserLogin>,
      IUserLoginRepository
{
    public UserLoginRepository(WmsDbContext context)
        : base(context)
    {
    }

    public async Task<UserLogin?> GetByUsernameAsync(
        string username)
    {
        return await _context.UserLogins
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u =>
                u.Username == username);
    }
}
