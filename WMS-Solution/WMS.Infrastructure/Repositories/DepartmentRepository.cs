using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Persistence;

namespace WMS.Infrastructure.Repositories;

public class DepartmentRepository
    : GenericRepository<Department>,
      IDepartmentRepository
{
    public DepartmentRepository(
        WmsDbContext context)
        : base(context)
    {
    }
}
