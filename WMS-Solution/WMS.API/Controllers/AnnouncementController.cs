using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.Announcement;
using WMS.Application.Interfaces.Services;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnnouncementController
    : ControllerBase
{
    private readonly IAnnouncementService
        _announcementService;

    public AnnouncementController(
        IAnnouncementService
            announcementService)
    {
        _announcementService =
            announcementService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result =
            await _announcementService
                .GetAllAsync();

        return Ok(
            ApiResponse<IEnumerable<
                AnnouncementResponseDto>>
            .SuccessResponse(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        CreateAnnouncementDto dto)
    {
        var result =
            await _announcementService
                .CreateAsync(dto);

        return Ok(
            ApiResponse<
                AnnouncementResponseDto>
            .SuccessResponse(result));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        int id,
        CreateAnnouncementDto dto)
    {
        var result =
            await _announcementService.UpdateAsync(id, dto);

        return Ok(
            ApiResponse<AnnouncementResponseDto>
            .SuccessResponse(result));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _announcementService.DeleteAsync(id);

        return Ok(
            ApiResponse<string>
                .SuccessResponse(string.Empty, "Announcement deleted successfully."));
    }
}
