using System;
using System.Linq;
using WMS.Application.DTOs.Dashboard;
using WMS.Application.Interfaces.Services;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

public class DashboardService
    : IDashboardService
{
    private readonly IDashboardRepository
        _dashboardRepository;

    private readonly IUnitOfWork
        _unitOfWork;

    public DashboardService(
        IDashboardRepository
            dashboardRepository,
        IUnitOfWork unitOfWork)
    {
        _dashboardRepository =
            dashboardRepository;

        _unitOfWork = unitOfWork;
    }

    public async Task<DashboardResponseDto>
        GetDashboardAsync(
            string role,
            int? employeeId,
            int? departmentId)
    {
        if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            return new DashboardResponseDto
            {
                TotalEmployees = await _dashboardRepository.GetTotalEmployeesAsync(),
                TotalDepartments = await _dashboardRepository.GetTotalDepartmentsAsync(),
                TotalProjects = await _dashboardRepository.GetTotalProjectsAsync(),
                PendingLeaves = await _dashboardRepository.GetPendingLeavesAsync(),
                ActiveAnnouncements = await _dashboardRepository.GetActiveAnnouncementsAsync(),
                TotalHours = await GetAttendanceHoursAsync()
            };
        }

        if (string.Equals(role, "Manager", StringComparison.OrdinalIgnoreCase) && departmentId.HasValue)
        {
            var employees = (await _unitOfWork.Employees.GetAllAsync())
                .Where(employee => employee.DepartmentId == departmentId.Value)
                .ToList();

            var teamEmployeeIds = employees
                .Select(employee => employee.EmployeeId)
                .ToHashSet();

            var allocations = await _unitOfWork.Allocations.GetAllAsync();
            var leaves = await _unitOfWork.Leaves.GetAllAsync();
            var attendances = await _unitOfWork.Attendances.GetAllAsync();

            return new DashboardResponseDto
            {
                TotalEmployees = employees.Count,
                TotalDepartments = 1,
                TotalProjects = allocations.Where(allocation => teamEmployeeIds.Contains(allocation.EmpId)).Select(allocation => allocation.ProjectId).Distinct().Count(),
                PendingLeaves = leaves.Count(leave => teamEmployeeIds.Contains(leave.EmpId) && leave.Status == LeaveStatus.Pending),
                ActiveAnnouncements = await _dashboardRepository.GetActiveAnnouncementsAsync(),
                TotalHours = attendances.Where(attendance => teamEmployeeIds.Contains(attendance.EmpId)).Sum(attendance => attendance.TotalHours)
            };
        }

        if (string.Equals(role, "Employee", StringComparison.OrdinalIgnoreCase) && employeeId.HasValue)
        {
            var allocations = await _unitOfWork.Allocations.GetAllAsync();
            var leaves = await _unitOfWork.Leaves.GetAllAsync();
            var attendances = await _unitOfWork.Attendances.GetAllAsync();

            return new DashboardResponseDto
            {
                TotalEmployees = 1,
                TotalDepartments = 1,
                TotalProjects = allocations.Where(allocation => allocation.EmpId == employeeId.Value).Select(allocation => allocation.ProjectId).Distinct().Count(),
                PendingLeaves = leaves.Count(leave => leave.EmpId == employeeId.Value && leave.Status == LeaveStatus.Pending),
                ActiveAnnouncements = await _dashboardRepository.GetActiveAnnouncementsAsync(),
                TotalHours = attendances.Where(attendance => attendance.EmpId == employeeId.Value).Sum(attendance => attendance.TotalHours)
            };
        }

        return new DashboardResponseDto
        {
            TotalEmployees =
                await _dashboardRepository
                    .GetTotalEmployeesAsync(),

            TotalDepartments =
                await _dashboardRepository
                    .GetTotalDepartmentsAsync(),

            TotalProjects =
                await _dashboardRepository
                    .GetTotalProjectsAsync(),

            PendingLeaves =
                await _dashboardRepository
                    .GetPendingLeavesAsync(),

            ActiveAnnouncements =
                await _dashboardRepository
                    .GetActiveAnnouncementsAsync(),

            TotalHours = await GetAttendanceHoursAsync()
        };
    }

    private async Task<double> GetAttendanceHoursAsync()
    {
        var attendances = await _unitOfWork.Attendances.GetAllAsync();

        return attendances.Sum(attendance => attendance.TotalHours);
    }
}
