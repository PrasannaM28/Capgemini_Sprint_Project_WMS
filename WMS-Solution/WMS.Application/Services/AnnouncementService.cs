using AutoMapper;
using WMS.Application.DTOs.Announcement;
using WMS.Application.Interfaces.Services;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

public class AnnouncementService
    : IAnnouncementService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    public AnnouncementService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;

        _mapper = mapper;
    }

    public async Task<AnnouncementResponseDto>
        CreateAsync(CreateAnnouncementDto dto)
    {
        var announcement =
            new Announcement
            {
                Title = dto.Title,

                Message = dto.Message,

                CreatedBy = dto.CreatedBy,

                CreatedOn = DateTime.UtcNow,

                IsActive = true
            };

        await _unitOfWork
            .Announcements
            .AddAsync(announcement);

        await _unitOfWork
            .SaveChangesAsync();

        return _mapper.Map<
            AnnouncementResponseDto>(
                announcement);
    }

    public async Task<AnnouncementResponseDto>
        UpdateAsync(int announcementId, CreateAnnouncementDto dto)
    {
        var announcement =
            await _unitOfWork
                .Announcements
                .GetByIdAsync(announcementId);

        if (announcement == null)
        {
            throw new KeyNotFoundException(
                "Announcement not found.");
        }

        announcement.Title = dto.Title;
        announcement.Message = dto.Message;
        announcement.IsActive = true;
        announcement.UpdatedOn = DateTime.UtcNow;

        _unitOfWork.Announcements.Update(announcement);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AnnouncementResponseDto>(announcement);
    }

    public async Task DeleteAsync(int announcementId)
    {
        var announcement =
            await _unitOfWork
                .Announcements
                .GetByIdAsync(announcementId);

        if (announcement == null)
        {
            throw new KeyNotFoundException(
                "Announcement not found.");
        }

        _unitOfWork.Announcements.Remove(announcement);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<
        AnnouncementResponseDto>>
        GetAllAsync()
    {
        var announcements =
            await _unitOfWork
                .Announcements
                .GetAllAsync();

        return _mapper.Map<
            IEnumerable<
                AnnouncementResponseDto>>
            (announcements);
    }
}
