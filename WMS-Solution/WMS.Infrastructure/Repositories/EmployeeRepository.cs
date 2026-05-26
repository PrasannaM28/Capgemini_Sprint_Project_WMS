using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Persistence;

namespace WMS.Infrastructure.Repositories;

public class EmployeeRepository
    : GenericRepository<Employee>,
      IEmployeeRepository
{
    public EmployeeRepository(WmsDbContext context)
        : base(context)
    {
    }

    public async Task<Employee?> GetEmployeeWithDepartmentAsync(
    int employeeId)
    {
        return await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Role)
            .FirstOrDefaultAsync(e =>
                e.EmployeeId == employeeId);
    }

    public async Task<IEnumerable<Employee>>
    GetAllEmployeesWithDetailsAsync()
    {
        return await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Role)
            .ToListAsync();
    }

    public async Task<IEnumerable<Employee>> SearchEmployeesAsync(
        string searchTerm)
    {
        searchTerm = searchTerm.ToLower();

        var isIdSearch = int.TryParse(searchTerm, out var employeeId);

        return await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Role)
            .Where(e =>
                e.FirstName.ToLower().Contains(searchTerm)
                || e.LastName.ToLower().Contains(searchTerm)
                || e.Email.ToLower().Contains(searchTerm)
                || (e.Department != null && e.Department.DepartmentName.ToLower().Contains(searchTerm))
                || (e.Role != null && e.Role.RoleName.ToLower().Contains(searchTerm))
                || e.EmployeeId == (isIdSearch ? employeeId : -1))
            .ToListAsync();
    }
}
