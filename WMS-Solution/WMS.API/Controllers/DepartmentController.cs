using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.Department;
using WMS.Application.Interfaces.Services;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentService
        _departmentService;

    public DepartmentController(
        IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result =
            await _departmentService.GetAllAsync();

        return Ok(
            ApiResponse<IEnumerable<
                DepartmentResponseDto>>
            .SuccessResponse(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        CreateDepartmentDto dto)
    {
        var result =
            await _departmentService
                .CreateAsync(dto);

        return Ok(
            ApiResponse<DepartmentResponseDto>
            .SuccessResponse(result));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] CreateDepartmentDto dto)
    {
        var result =
            await _departmentService.UpdateAsync(id, dto);

        return Ok(
            ApiResponse<DepartmentResponseDto>
            .SuccessResponse(result));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _departmentService.DeleteAsync(id);

        return Ok(
            ApiResponse<string>
                .SuccessResponse(
                    string.Empty,
                    "Department deleted successfully."));
    }
}
