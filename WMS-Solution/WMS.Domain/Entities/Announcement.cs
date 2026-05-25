using System.ComponentModel.DataAnnotations;
using WMS.Domain.Common;

namespace WMS.Domain.Entities;

public class Announcement : BaseEntity
{
    [Key]
    public int AnnouncementId { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;

    [Required]
    public int CreatedBy { get; set; }

    public bool IsActive { get; set; } = true;
}
