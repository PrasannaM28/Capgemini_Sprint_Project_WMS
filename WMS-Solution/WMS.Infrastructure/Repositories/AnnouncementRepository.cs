using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Persistence;

namespace WMS.Infrastructure.Repositories;

public class AnnouncementRepository
    : GenericRepository<Announcement>,
      IAnnouncementRepository
{
    public AnnouncementRepository(
        WmsDbContext context)
        : base(context)
    {
    }
}
