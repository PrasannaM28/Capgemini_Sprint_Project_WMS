using WMS.Domain.Enums;

namespace WMS.Application.DTOs.Project;

public class CreateProjectDto
{
    public string ProjectName { get; set; }
        = string.Empty;

    public int? ClientId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public ProjectStatus? Status { get; set; }
}
