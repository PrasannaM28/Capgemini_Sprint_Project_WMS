using AutoMapper;
using WMS.Application.DTOs.Allocation;
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
        var allocation =
            new EmployeeProjectAllocation
            {
                EmpId = dto.EmpId,

                ProjectId = dto.ProjectId,

                AssignedOn = DateTime.UtcNow,

                CreateDate = DateTime.UtcNow,

                CreatedBy = dto.CreatedBy
            };

        await _unitOfWork
            .Allocations
            .AddAsync(allocation);

        await _unitOfWork
            .SaveChangesAsync();

        return _mapper.Map<
            AllocationResponseDto>(allocation);
    }
}
