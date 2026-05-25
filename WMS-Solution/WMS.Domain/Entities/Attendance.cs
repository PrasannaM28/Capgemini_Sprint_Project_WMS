using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WMS.Domain.Common;
using WMS.Domain.Enums;

namespace WMS.Domain.Entities;

public class Attendance : BaseEntity
{
    [Key]
    public int AttendanceId { get; set; }

    [Required]
    public int EmpId { get; set; }

    [ForeignKey(nameof(EmpId))]
    public Employee? Employee { get; set; }

    [Required]
    public DateTime CheckIn { get; set; }

    public DateTime? CheckOut { get; set; }

    public double TotalHours { get; set; }

    public WorkMode WorkMode { get; set; }

    [Required]
    public DateTime AttendanceDate { get; set; }
}
