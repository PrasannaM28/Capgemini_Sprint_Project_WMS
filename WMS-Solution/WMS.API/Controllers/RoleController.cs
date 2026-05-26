using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.Role;
using WMS.Domain.Interfaces;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoleController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public RoleController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var roles = await _unitOfWork.Roles.GetAllAsync();

        var result = roles
            .Select(role => new RoleResponseDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName
            })
            .OrderBy(role => role.RoleName)
            .ToList();

        return Ok(ApiResponse<IEnumerable<RoleResponseDto>>.SuccessResponse(result));
    }
}