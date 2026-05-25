using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Persistence;

namespace WMS.Infrastructure.Repositories;

public class AllocationRepository
    : GenericRepository<EmployeeProjectAllocation>,
      IAllocationRepository
{
    public AllocationRepository(
        WmsDbContext context)
        : base(context)
    {
    }
}
