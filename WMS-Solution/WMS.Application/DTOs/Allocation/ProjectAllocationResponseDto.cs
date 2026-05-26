namespace WMS.Application.DTOs.Allocation;

public class ProjectAllocationResponseDto
{
    public int AllocationId { get; set; }

    public int EmpId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public int ProjectId { get; set; }

    public string ProjectName { get; set; } = string.Empty;

    public DateTime AssignedOn { get; set; }
}