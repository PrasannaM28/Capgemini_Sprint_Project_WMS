namespace WMS.Application.DTOs.Announcement;

public class CreateAnnouncementDto
{
    public string Title { get; set; }
        = string.Empty;

    public string Message { get; set; }
        = string.Empty;

    public int CreatedBy { get; set; }
}
