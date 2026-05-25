using BCrypt.Net;
using Microsoft.Extensions.Logging;
using WMS.Application.DTOs.Auth;
using WMS.Application.Exceptions;
using WMS.Application.Interfaces.Services;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

public class AuthService : IAuthService
{
    private const int MinimumPasswordLength = 6;

    private readonly IUnitOfWork
        _unitOfWork;

    private readonly IJwtTokenGenerator
        _jwtTokenGenerator;

    private readonly ILogger<AuthService>
        _logger;

    public AuthService(
        IUnitOfWork unitOfWork,
        IJwtTokenGenerator jwtTokenGenerator,
        ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork;

        _jwtTokenGenerator =
            jwtTokenGenerator;

        _logger = logger;
    }

    public async Task<LoginResponseDto>
        LoginAsync(
            LoginRequestDto dto)
    {
        var normalizedUsername =
            dto.Username
                .Trim();

        var user =
            await _unitOfWork
                .UserLogins
                .GetByUsernameAsync(normalizedUsername);

        if (user == null)
        {
            var users =
                await _unitOfWork
                    .UserLogins
                    .FindAsync(u =>
                        u.Username.ToLower() ==
                        normalizedUsername.ToLower());

            user = users.FirstOrDefault();
        }

        if (user == null)
        {
            _logger.LogWarning(
                "Invalid login attempt for username: {Username}",
                normalizedUsername);

            throw new BadRequestException(
                "Invalid username or password.");
        }

        var passwordValid =
            BCrypt.Net.BCrypt.Verify(
                dto.Password,
                user.PasswordHash);

        if (!passwordValid)
        {
            _logger.LogWarning(
                "Invalid password attempt for username: {Username}",
                normalizedUsername);

            throw new BadRequestException(
                "Invalid username or password.");
        }

        var role =
            await _unitOfWork
                .Roles
                .GetByIdAsync(user.RoleId);

        var token =
            _jwtTokenGenerator
                .GenerateToken(
                    user,
                    role!.RoleName);

        _logger.LogInformation(
            "User logged in successfully: {Username}",
            user.Username);

        return new LoginResponseDto
        {
            Token = token,

            Expiration =
                DateTime.UtcNow
                    .AddMinutes(60),

            Username =
                user.Username,

            Role =
                role.RoleName
        };
    }

    public async Task ChangePasswordAsync(
        int userId,
        ChangePasswordRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CurrentPassword) ||
            string.IsNullOrWhiteSpace(dto.NewPassword) ||
            string.IsNullOrWhiteSpace(dto.ConfirmNewPassword))
        {
            throw new BadRequestException(
                "All password fields are required.");
        }

        if (dto.NewPassword != dto.ConfirmNewPassword)
        {
            throw new BadRequestException(
                "New password and confirmation do not match.");
        }

        if (dto.NewPassword.Length < MinimumPasswordLength)
        {
            throw new BadRequestException(
                "New password must be at least 6 characters long.");
        }

        var user = await _unitOfWork
            .UserLogins
            .GetByIdAsync(userId);

        if (user == null)
        {
            throw new NotFoundException(
                "User account not found.");
        }

        var currentPasswordValid =
            BCrypt.Net.BCrypt.Verify(
                dto.CurrentPassword,
                user.PasswordHash);

        if (!currentPasswordValid)
        {
            throw new BadRequestException(
                "Current password is invalid.");
        }

        user.PasswordHash = BCrypt.Net.BCrypt
            .HashPassword(dto.NewPassword);

        _unitOfWork.UserLogins.Update(user);

        await _unitOfWork.SaveChangesAsync();
    }
}
