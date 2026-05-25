using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Persistence;

namespace WMS.Infrastructure.Repositories;

public class LeaveRepository
    : GenericRepository<Leave>,
      ILeaveRepository
{
    public LeaveRepository(
        WmsDbContext context)
        : base(context)
    {
    }
}
