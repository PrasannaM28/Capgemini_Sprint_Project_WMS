using AutoMapper;
using Microsoft.Extensions.Logging;
using WMS.Application.DTOs.Leave;
using WMS.Application.Exceptions;
using WMS.Application.Interfaces.Services;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Domain.Enums;

namespace WMS.Application.Services;

public class LeaveService
    : ILeaveService
{
    private readonly IUnitOfWork
        _unitOfWork;

    private readonly IMapper _mapper;

    private readonly ILogger<LeaveService>
        _logger;

    public LeaveService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<LeaveService> logger)
    {
        _unitOfWork = unitOfWork;

        _mapper = mapper;

        _logger = logger;
    }

    public async Task<IEnumerable<LeaveResponseDto>>
        GetAllAsync()
    {
        var leaves =
            await _unitOfWork
                .Leaves
                .GetAllAsync();

        return _mapper.Map<IEnumerable<LeaveResponseDto>>(leaves);
    }

    public async Task<LeaveResponseDto?>
        GetByIdAsync(int leaveId)
    {
        var leave =
            await _unitOfWork
                .Leaves
                .GetByIdAsync(leaveId);

        return leave == null
            ? null
            : _mapper.Map<LeaveResponseDto>(leave);
    }

    public async Task<LeaveResponseDto>
        ApplyLeaveAsync(
            CreateLeaveDto dto)
    {
        var leave = new Leave
        {
            EmpId = dto.EmpId,

            LeaveType = dto.LeaveType,

            Reason = dto.Reason,

            FromDate = dto.FromDate,

            ToDate = dto.ToDate,

            Status = LeaveStatus.Pending
        };

        await _unitOfWork
            .Leaves
            .AddAsync(leave);

        await _unitOfWork
            .SaveChangesAsync();

        _logger.LogInformation(
            "Leave applied successfully for Employee Id: {EmpId}",
            dto.EmpId);

        return _mapper.Map<
            LeaveResponseDto>(leave);
    }

    public async Task<LeaveResponseDto>
        ApproveLeaveAsync(
            LeaveApprovalDto dto)
    {
        var leave =
            await _unitOfWork
                .Leaves
                .GetByIdAsync(dto.LeaveId);

        if (leave == null)
        {
            _logger.LogWarning(
                "Leave not found with Id: {LeaveId}",
                dto.LeaveId);

            throw new NotFoundException(
                "Leave not found.");
        }

        leave.Status = dto.Status;

        leave.ApprovedBy =
            dto.ApprovedBy;

        leave.ApprovedOn =
            DateTime.UtcNow;

        _unitOfWork
            .Leaves
            .Update(leave);

        await _unitOfWork
            .SaveChangesAsync();

        _logger.LogInformation(
            "Leave approved successfully with Leave Id: {LeaveId}",
            leave.LeaveId);

        return _mapper.Map<
            LeaveResponseDto>(leave);
    }

    public async Task CancelLeaveAsync(
        int leaveId)
    {
        var leave =
            await _unitOfWork
                .Leaves
                .GetByIdAsync(leaveId);

        if (leave == null)
        {
            _logger.LogWarning(
                "Leave not found for cancel with Id: {LeaveId}",
                leaveId);

            throw new NotFoundException(
                "Leave not found.");
        }

        leave.Status =
            LeaveStatus.Cancelled;

        _unitOfWork
            .Leaves
            .Update(leave);

        await _unitOfWork
            .SaveChangesAsync();

        _logger.LogInformation(
            "Leave cancelled successfully with Leave Id: {LeaveId}",
            leave.LeaveId);
    }
}
