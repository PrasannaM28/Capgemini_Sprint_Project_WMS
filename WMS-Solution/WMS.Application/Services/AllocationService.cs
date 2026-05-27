using AutoMapper;
using WMS.Application.DTOs.Allocation;
using WMS.Application.Exceptions;
using WMS.Application.Interfaces.Services;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

public class AllocationService
    : IAllocationService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    public AllocationService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;

        _mapper = mapper;
    }

    public async Task<AllocationResponseDto>
        AllocateAsync(CreateAllocationDto dto)
    {
        var existingAllocation = (await _unitOfWork.Allocations.FindAsync(allocation =>
                allocation.EmpId == dto.EmpId && allocation.ProjectId == dto.ProjectId))
            .FirstOrDefault();

        var now = DateTime.UtcNow;

        EmployeeProjectAllocation allocation;

        if (existingAllocation is null)
        {
            allocation = new EmployeeProjectAllocation
            {
                EmpId = dto.EmpId,
                ProjectId = dto.ProjectId,
                AssignedOn = now,
                CreateDate = now,
                CreatedBy = dto.CreatedBy,
                Status = true
            };

            await _unitOfWork.Allocations.AddAsync(allocation);
        }
        else
        {
            existingAllocation.AssignedOn = now;
            existingAllocation.Status = true;
            existingAllocation.UpdatedBy = dto.CreatedBy;
            existingAllocation.UpdatedDate = now;

            _unitOfWork.Allocations.Update(existingAllocation);

            allocation = existingAllocation;
        }

        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception exception) when (IsDuplicateAllocation(exception))
        {
            throw new BadRequestException(
                "This employee is already assigned to this project.");
        }

        return _mapper.Map<
            AllocationResponseDto>(allocation);
    }

    private static bool IsDuplicateAllocation(
        Exception exception)
    {
        var message = exception.InnerException?.Message
            ?? exception.Message;

        return message.Contains(
                "Cannot insert duplicate key row",
                StringComparison.OrdinalIgnoreCase)
            || message.Contains(
                "IX_EmployeeProjectAllocations_EmpId_ProjectId",
                StringComparison.OrdinalIgnoreCase)
            || message.Contains(
                "Violation of UNIQUE KEY constraint",
                StringComparison.OrdinalIgnoreCase);
    }
}
