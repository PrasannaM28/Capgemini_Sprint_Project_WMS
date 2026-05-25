using WMS.Application.DTOs.Allocation;

namespace WMS.Application.Interfaces.Services;

public interface IAllocationService
{
    Task<AllocationResponseDto>
        AllocateAsync(CreateAllocationDto dto);
}
