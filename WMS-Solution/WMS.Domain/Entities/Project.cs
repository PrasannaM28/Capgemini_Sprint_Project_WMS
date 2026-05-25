using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WMS.Domain.Common;
using WMS.Domain.Enums;

namespace WMS.Domain.Entities;

public class Project : BaseEntity
{
    [Key]
    public int ProjectId { get; set; }

    [Required]
    [StringLength(100)]
    public string ProjectName { get; set; } = string.Empty;

    public int? ClientId { get; set; }

    [ForeignKey(nameof(ClientId))]
    public Client? Client { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public ProjectStatus Status { get; set; }
        = ProjectStatus.Active;

    // Navigation Property
    public ICollection<EmployeeProjectAllocation> Allocations { get; set; }
        = new List<EmployeeProjectAllocation>();
}
