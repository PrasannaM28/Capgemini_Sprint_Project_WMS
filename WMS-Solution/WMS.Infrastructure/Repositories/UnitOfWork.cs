using WMS.Domain.Interfaces;
using WMS.Infrastructure.Persistence;

namespace WMS.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly WmsDbContext _context;

    public IEmployeeRepository Employees { get; }

    public IUserLoginRepository UserLogins { get; }

    public IDepartmentRepository Departments { get; }

    public IAttendanceRepository Attendances { get; }

    public ILeaveRepository Leaves { get; }

    public IProjectRepository Projects { get; }

    public IClientRepository Clients { get; }

    public IAllocationRepository Allocations { get; }

    public IAnnouncementRepository Announcements { get; }

    public IRoleRepository Roles { get; }

    public IReportRepository Reports { get; }

    public UnitOfWork(WmsDbContext context)
    {
        _context = context;

        Employees = new EmployeeRepository(_context);

        UserLogins = new UserLoginRepository(_context);

        Departments = new DepartmentRepository(_context);

        Attendances = new AttendanceRepository(_context);

        Leaves = new LeaveRepository(_context);

        Projects = new ProjectRepository(_context);

        Clients = new ClientRepository(_context);

        Allocations = new AllocationRepository(_context);

        Announcements = new AnnouncementRepository(_context);

        Roles = new RoleRepository(_context);

        Reports = new ReportRepository(_context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
