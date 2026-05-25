using WMS.Application.DTOs.Client;

namespace WMS.Application.Interfaces.Services;

public interface IClientService
{
    Task<ClientResponseDto>
        CreateAsync(CreateClientDto dto);

    Task<ClientResponseDto>
        UpdateAsync(int clientId, CreateClientDto dto);

    Task DeleteAsync(int clientId);

    Task<IEnumerable<ClientResponseDto>>
        GetAllAsync();
}
