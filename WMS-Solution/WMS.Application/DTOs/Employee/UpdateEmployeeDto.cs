using WMS.Domain.Enums;

namespace WMS.Application.DTOs.Employee;

public class UpdateEmployeeDto
{
    public int EmployeeId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public Gender Gender { get; set; }

    public int DepartmentId { get; set; }

    public int RoleId { get; set; }
}
