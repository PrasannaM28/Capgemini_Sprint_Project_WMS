using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<Employee?> GetEmployeeWithDepartmentAsync(int employeeId);

    Task<IEnumerable<Employee>> SearchEmployeesAsync(string searchTerm);

    Task<IEnumerable<Employee>> GetAllEmployeesWithDetailsAsync();
}
