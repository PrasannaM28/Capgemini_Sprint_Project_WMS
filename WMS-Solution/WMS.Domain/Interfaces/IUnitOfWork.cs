namespace WMS.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IEmployeeRepository Employees { get; }

    IUserLoginRepository UserLogins { get; }

    IDepartmentRepository Departments { get; }

    IAttendanceRepository Attendances { get; }

    ILeaveRepository Leaves { get; }

    IProjectRepository Projects { get; }

    IClientRepository Clients { get; }

    IAllocationRepository Allocations { get; }

    IAnnouncementRepository Announcements { get; }

    IRoleRepository Roles { get; }

    IReportRepository Reports { get; }

    Task<int> SaveChangesAsync();
}
