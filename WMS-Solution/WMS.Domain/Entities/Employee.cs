using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WMS.Domain.Common;
using WMS.Domain.Enums;

namespace WMS.Domain.Entities;

public class Employee : BaseEntity
{
    [Key]
    public int EmployeeId { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(15)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public Gender Gender { get; set; }

    [Required]
    public DateTime DOB { get; set; }

    [Required]
    public DateTime DOJ { get; set; }

    [Required]
    public EmployeeStatus Status { get; set; }
        = EmployeeStatus.Active;

    [ForeignKey(nameof(Department))]
    public int DepartmentId { get; set; }

    [ForeignKey(nameof(Role))]
    public int RoleId { get; set; }

    // Navigation Properties
    public Department? Department { get; set; }

    public Role? Role { get; set; }

    public ICollection<Attendance> Attendances { get; set; }
        = new List<Attendance>();

    public ICollection<Leave> Leaves { get; set; }
        = new List<Leave>();

    public ICollection<EmployeeProjectAllocation> Allocations { get; set; }
        = new List<EmployeeProjectAllocation>();
}
