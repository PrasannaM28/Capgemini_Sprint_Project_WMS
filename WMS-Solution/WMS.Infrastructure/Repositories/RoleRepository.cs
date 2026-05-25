using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Persistence;

namespace WMS.Infrastructure.Repositories;

public class RoleRepository
    : GenericRepository<Role>,
      IRoleRepository
{
    public RoleRepository(
        WmsDbContext context)
        : base(context)
    {
    }
}
