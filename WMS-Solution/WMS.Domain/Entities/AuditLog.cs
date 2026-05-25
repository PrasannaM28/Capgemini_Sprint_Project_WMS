using System.ComponentModel.DataAnnotations;
using WMS.Domain.Common;

namespace WMS.Domain.Entities;

public class AuditLog : BaseEntity
{
    [Key]
    public int AuditId { get; set; }

    [Required]
    public string EntityName { get; set; } = string.Empty;

    [Required]
    public int RecordId { get; set; }

    [Required]
    [StringLength(20)]
    public string Action { get; set; } = string.Empty;

    [Required]
    public int CreatedBy { get; set; }
}
