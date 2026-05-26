using System;
using System.Collections.Generic;
using System.Linq;
using WMS.Application.DTOs.Dashboard;
using WMS.Application.Interfaces.Services;
using WMS.Domain.Entities;
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
        var employees = await _unitOfWork.Employees.GetAllAsync();
        var attendances = await _unitOfWork.Attendances.GetAllAsync();
        var leaves = await _unitOfWork.Leaves.GetAllAsync();
        var projects = await _unitOfWork.Projects.GetAllAsync();
        var allocations = await _unitOfWork.Allocations.GetAllAsync();
        var clients = await _unitOfWork.Clients.GetAllAsync();

        if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            var activeEmployees = employees
                .Where(employee => employee.Status == EmployeeStatus.Active)
                .ToList();

            var activeEmployeeIds = activeEmployees
                .Select(employee => employee.EmployeeId)
                .ToHashSet();

            var presentToday = GetPresentTodayCount(attendances, activeEmployeeIds);

            return new DashboardResponseDto
            {
                TotalEmployees = activeEmployees.Count,
                TotalDepartments = await _dashboardRepository.GetTotalDepartmentsAsync(),
                TotalClients = clients.Count(),
                TotalProjects = projects.Count(),
                ActiveProjects = projects.Count(project => project.Status == ProjectStatus.Active),
                CompletedProjects = projects.Count(project => project.Status == ProjectStatus.Completed),
                PresentToday = presentToday,
                AbsentToday = Math.Max(0, activeEmployees.Count - presentToday),
                PendingLeaves = leaves.Count(leave => leave.Status == LeaveStatus.Pending),
                ApprovedLeaves = leaves.Count(leave => leave.Status == LeaveStatus.Approved),
                RejectedLeaves = leaves.Count(leave => leave.Status == LeaveStatus.Rejected),
                ActiveAnnouncements = await _dashboardRepository.GetActiveAnnouncementsAsync(),
                TotalHours = attendances.Sum(attendance => attendance.TotalHours),
                AttendanceLast7Days = BuildLast7DaysTrend(attendances, activeEmployeeIds)
            };
        }

        if (string.Equals(role, "Manager", StringComparison.OrdinalIgnoreCase) && departmentId.HasValue)
        {
            var teamEmployees = employees
                .Where(employee => employee.DepartmentId == departmentId.Value)
                .ToList();

            var teamEmployeeIds = teamEmployees
                .Select(employee => employee.EmployeeId)
                .ToHashSet();

            var teamProjectIds = allocations
                .Where(allocation => teamEmployeeIds.Contains(allocation.EmpId))
                .Select(allocation => allocation.ProjectId)
                .Distinct()
                .ToHashSet();

            var presentToday = GetPresentTodayCount(attendances, teamEmployeeIds);
            var ownAttendance = GetOwnAttendanceProgress(attendances, employeeId);

            return new DashboardResponseDto
            {
                TotalEmployees = teamEmployees.Count,
                TotalDepartments = 1,
                TotalProjects = teamProjectIds.Count,
                ActiveProjects = projects.Count(project => teamProjectIds.Contains(project.ProjectId) && project.Status == ProjectStatus.Active),
                CompletedProjects = projects.Count(project => teamProjectIds.Contains(project.ProjectId) && project.Status == ProjectStatus.Completed),
                PresentToday = presentToday,
                AbsentToday = Math.Max(0, teamEmployees.Count - presentToday),
                PendingLeaves = leaves.Count(leave => teamEmployeeIds.Contains(leave.EmpId) && leave.Status == LeaveStatus.Pending),
                ApprovedLeaves = leaves.Count(leave => teamEmployeeIds.Contains(leave.EmpId) && leave.Status == LeaveStatus.Approved),
                RejectedLeaves = leaves.Count(leave => teamEmployeeIds.Contains(leave.EmpId) && leave.Status == LeaveStatus.Rejected),
                ActiveAnnouncements = await _dashboardRepository.GetActiveAnnouncementsAsync(),
                TotalHours = attendances.Where(attendance => teamEmployeeIds.Contains(attendance.EmpId)).Sum(attendance => attendance.TotalHours),
                OwnAttendancePercent = ownAttendance.percent,
                OwnPresentDays = ownAttendance.presentDays,
                OwnAttendanceTargetDays = ownAttendance.targetDays
            };
        }

        if (string.Equals(role, "Employee", StringComparison.OrdinalIgnoreCase) && employeeId.HasValue)
        {
            var ownProjectIds = allocations
                .Where(allocation => allocation.EmpId == employeeId.Value)
                .Select(allocation => allocation.ProjectId)
                .Distinct()
                .ToHashSet();

            var ownAttendance = GetOwnAttendanceProgress(attendances, employeeId);
            var presentToday = GetPresentTodayCount(attendances, new HashSet<int> { employeeId.Value });

            return new DashboardResponseDto
            {
                TotalEmployees = 1,
                TotalDepartments = 1,
                TotalProjects = ownProjectIds.Count,
                ActiveProjects = projects.Count(project => ownProjectIds.Contains(project.ProjectId) && project.Status == ProjectStatus.Active),
                CompletedProjects = projects.Count(project => ownProjectIds.Contains(project.ProjectId) && project.Status == ProjectStatus.Completed),
                PresentToday = presentToday,
                AbsentToday = presentToday > 0 ? 0 : 1,
                PendingLeaves = leaves.Count(leave => leave.EmpId == employeeId.Value && leave.Status == LeaveStatus.Pending),
                ApprovedLeaves = leaves.Count(leave => leave.EmpId == employeeId.Value && leave.Status == LeaveStatus.Approved),
                RejectedLeaves = leaves.Count(leave => leave.EmpId == employeeId.Value && leave.Status == LeaveStatus.Rejected),
                ActiveAnnouncements = await _dashboardRepository.GetActiveAnnouncementsAsync(),
                TotalHours = attendances.Where(attendance => attendance.EmpId == employeeId.Value).Sum(attendance => attendance.TotalHours),
                OwnAttendancePercent = ownAttendance.percent,
                OwnPresentDays = ownAttendance.presentDays,
                OwnAttendanceTargetDays = ownAttendance.targetDays
            };
        }

        var fallbackPresentToday = GetPresentTodayCount(attendances, null);

        return new DashboardResponseDto
        {
            TotalEmployees = employees.Count(),
            TotalDepartments = await _dashboardRepository.GetTotalDepartmentsAsync(),
            TotalClients = clients.Count(),
            TotalProjects = projects.Count(),
            ActiveProjects = projects.Count(project => project.Status == ProjectStatus.Active),
            CompletedProjects = projects.Count(project => project.Status == ProjectStatus.Completed),
            PresentToday = fallbackPresentToday,
            AbsentToday = Math.Max(0, employees.Count() - fallbackPresentToday),
            PendingLeaves = leaves.Count(leave => leave.Status == LeaveStatus.Pending),
            ApprovedLeaves = leaves.Count(leave => leave.Status == LeaveStatus.Approved),
            RejectedLeaves = leaves.Count(leave => leave.Status == LeaveStatus.Rejected),
            ActiveAnnouncements = await _dashboardRepository.GetActiveAnnouncementsAsync(),
            TotalHours = attendances.Sum(attendance => attendance.TotalHours),
            AttendanceLast7Days = BuildLast7DaysTrend(attendances, null)
        };
    }

    private static int GetPresentTodayCount(
        IEnumerable<Attendance> attendances,
        HashSet<int>? employeeIds)
    {
        var today = DateTime.Today;

        var query = attendances
            .Where(attendance => attendance.AttendanceDate.Date == today);

        if (employeeIds != null)
        {
            query = query.Where(attendance => employeeIds.Contains(attendance.EmpId));
        }

        return query
            .Select(attendance => attendance.EmpId)
            .Distinct()
            .Count();
    }

    private static List<DashboardAttendancePointDto> BuildLast7DaysTrend(
        IEnumerable<Attendance> attendances,
        HashSet<int>? employeeIds)
    {
        var today = DateTime.Today;
        var startDate = today.AddDays(-6);

        var filtered = attendances
            .Where(attendance =>
                attendance.AttendanceDate.Date >= startDate &&
                attendance.AttendanceDate.Date <= today);

        if (employeeIds != null)
        {
            filtered = filtered
                .Where(attendance => employeeIds.Contains(attendance.EmpId));
        }

        var groupedCounts = filtered
            .GroupBy(attendance => attendance.AttendanceDate.Date)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(attendance => attendance.EmpId)
                    .Distinct()
                    .Count());

        return Enumerable.Range(0, 7)
            .Select(offset =>
            {
                var date = startDate.AddDays(offset);

                return new DashboardAttendancePointDto
                {
                    Date = date,
                    PresentCount = groupedCounts.TryGetValue(date, out var count)
                        ? count
                        : 0
                };
            })
            .ToList();
    }

    private static (int percent, int presentDays, int targetDays)
        GetOwnAttendanceProgress(
            IEnumerable<Attendance> attendances,
            int? employeeId)
    {
        if (!employeeId.HasValue)
        {
            return (0, 0, 0);
        }

        var today = DateTime.Today;
        var monthStart = new DateTime(today.Year, today.Month, 1);

        var presentDays = attendances
            .Where(attendance =>
                attendance.EmpId == employeeId.Value &&
                attendance.AttendanceDate.Date >= monthStart &&
                attendance.AttendanceDate.Date <= today)
            .Select(attendance => attendance.AttendanceDate.Date)
            .Distinct()
            .Count();

        var targetDays = today.Day;

        if (targetDays <= 0)
        {
            return (0, presentDays, 0);
        }

        var percent = (int)Math.Round(
            (double)presentDays / targetDays * 100,
            MidpointRounding.AwayFromZero);

        return (Math.Clamp(percent, 0, 100), presentDays, targetDays);
    }
}
