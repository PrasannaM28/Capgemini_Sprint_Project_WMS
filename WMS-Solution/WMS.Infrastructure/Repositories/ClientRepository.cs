using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Persistence;

namespace WMS.Infrastructure.Repositories;

public class ClientRepository
    : GenericRepository<Client>,
      IClientRepository
{
    public ClientRepository(
        WmsDbContext context)
        : base(context)
    {
    }
}
