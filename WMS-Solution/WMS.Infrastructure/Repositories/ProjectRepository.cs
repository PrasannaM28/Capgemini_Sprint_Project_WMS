using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Persistence;

namespace WMS.Infrastructure.Repositories;

public class ProjectRepository
    : GenericRepository<Project>,
      IProjectRepository
{
    public ProjectRepository(
        WmsDbContext context)
        : base(context)
    {
    }
}
