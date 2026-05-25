using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.Auth;
using WMS.Application.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(
        IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto dto)
    {
        var result = await _authService
            .LoginAsync(dto);

        var response =
            ApiResponse<LoginResponseDto>
            .SuccessResponse(
                result,
                "Login successful.");

        return Ok(response);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequestDto dto)
    {
        var userIdValue = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized(
                ApiResponse<object>.FailureResponse(
                    new List<string>
                    {
                        "Unable to resolve the current user."
                    },
                    "Unauthorized."));
        }

        await _authService.ChangePasswordAsync(userId, dto);

        return Ok(
            ApiResponse<string>.SuccessResponse(
                "Password changed successfully.",
                "Password changed successfully."));
    }
}
