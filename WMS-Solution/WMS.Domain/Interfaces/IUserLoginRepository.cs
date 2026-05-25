using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IUserLoginRepository
    : IRepository<UserLogin>
{
    Task<UserLogin?> GetByUsernameAsync(
        string username);
}
