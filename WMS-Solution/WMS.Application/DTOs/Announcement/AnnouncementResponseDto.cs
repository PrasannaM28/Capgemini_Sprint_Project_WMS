namespace WMS.Application.DTOs.Announcement;

public class AnnouncementResponseDto
{
    public int AnnouncementId { get; set; }

    public string Title { get; set; }
        = string.Empty;

    public string Message { get; set; }
        = string.Empty;

    public DateTime CreatedOn { get; set; }

    public bool IsActive { get; set; }
}
