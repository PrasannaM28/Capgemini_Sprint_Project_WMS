using WMS.Application.DTOs.Announcement;

namespace WMS.Application.Interfaces.Services;

public interface IAnnouncementService
{
    Task<AnnouncementResponseDto>
        CreateAsync(CreateAnnouncementDto dto);

    Task<AnnouncementResponseDto>
        UpdateAsync(int announcementId, CreateAnnouncementDto dto);

    Task DeleteAsync(int announcementId);

    Task<IEnumerable<AnnouncementResponseDto>>
        GetAllAsync();
}
