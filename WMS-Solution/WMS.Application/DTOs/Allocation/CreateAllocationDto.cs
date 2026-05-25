namespace WMS.Application.DTOs.Allocation;

public class CreateAllocationDto
{
    public int EmpId { get; set; }

    public int ProjectId { get; set; }

    public string CreatedBy { get; set; }
        = string.Empty;
}
