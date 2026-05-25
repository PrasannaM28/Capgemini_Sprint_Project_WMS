using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WMS.Domain.Common;
using WMS.Domain.Enums;

namespace WMS.Domain.Entities;

public class Leave : BaseEntity
{
    [Key]
    public int LeaveId { get; set; }

    [Required]
    public int EmpId { get; set; }

    [ForeignKey(nameof(EmpId))]
    public Employee? Employee { get; set; }

    [Required]
    [StringLength(30)]
    public string LeaveType { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Reason { get; set; }

    [Required]
    public DateTime FromDate { get; set; }

    [Required]
    public DateTime ToDate { get; set; }

    [Required]
    public LeaveStatus Status { get; set; }
        = LeaveStatus.Pending;

    public DateTime AppliedOn { get; set; }
        = DateTime.UtcNow;

    public int? ApprovedBy { get; set; }

    public DateTime? ApprovedOn { get; set; }
}
