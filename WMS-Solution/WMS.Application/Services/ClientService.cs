using AutoMapper;
using WMS.Application.DTOs.Client;
using WMS.Application.Exceptions;
using WMS.Application.Interfaces.Services;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

public class ClientService : IClientService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    public ClientService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;

        _mapper = mapper;
    }

    public async Task<ClientResponseDto>
        CreateAsync(CreateClientDto dto)
    {
        var client =
            _mapper.Map<Client>(dto);

        await _unitOfWork
            .Clients
            .AddAsync(client);

        await _unitOfWork
            .SaveChangesAsync();

        return _mapper.Map<
            ClientResponseDto>(client);
    }

    public async Task<IEnumerable<ClientResponseDto>>
        GetAllAsync()
    {
        var clients =
            await _unitOfWork
                .Clients
                .GetAllAsync();

        return _mapper.Map<
            IEnumerable<ClientResponseDto>>
            (clients);
    }

    public async Task<ClientResponseDto>
        UpdateAsync(int clientId, CreateClientDto dto)
    {
        var client =
            await _unitOfWork
                .Clients
                .GetByIdAsync(clientId);

        if (client == null)
        {
            throw new NotFoundException("Client not found.");
        }

        client.ClientName = dto.ClientName;
        client.ClientAddress = dto.ClientAddress;
        client.ClientPhoneNumber = dto.ClientPhoneNumber;
        client.ClientLocation = dto.ClientLocation;

        _unitOfWork.Clients.Update(client);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ClientResponseDto>(client);
    }

    public async Task DeleteAsync(int clientId)
    {
        var client =
            await _unitOfWork
                .Clients
                .GetByIdAsync(clientId);

        if (client == null)
        {
            throw new NotFoundException("Client not found.");
        }

        _unitOfWork.Clients.Remove(client);
        await _unitOfWork.SaveChangesAsync();
    }
}
