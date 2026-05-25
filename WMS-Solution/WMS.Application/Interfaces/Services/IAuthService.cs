using WMS.Application.DTOs.Auth;

namespace WMS.Application.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(
        LoginRequestDto dto);

    Task ChangePasswordAsync(
        int userId,
        ChangePasswordRequestDto dto);
}
