using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.Client;
using WMS.Application.Interfaces.Services;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientController : ControllerBase
{
    private readonly IClientService
        _clientService;

    public ClientController(
        IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result =
            await _clientService.GetAllAsync();

        return Ok(
            ApiResponse<IEnumerable<
                ClientResponseDto>>
            .SuccessResponse(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        CreateClientDto dto)
    {
        var result =
            await _clientService
                .CreateAsync(dto);

        return Ok(
            ApiResponse<ClientResponseDto>
            .SuccessResponse(result));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] CreateClientDto dto)
    {
        var result =
            await _clientService
                .UpdateAsync(id, dto);

        return Ok(
            ApiResponse<ClientResponseDto>
            .SuccessResponse(result));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _clientService.DeleteAsync(id);

        return Ok(
            ApiResponse<string>
                .SuccessResponse(
                    string.Empty,
                    "Client deleted successfully."));
    }
}
