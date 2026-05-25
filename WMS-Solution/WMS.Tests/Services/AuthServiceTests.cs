using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WMS.Application.DTOs.Auth;
using WMS.Application.Exceptions;
using WMS.Application.Services;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork>
        _unitOfWorkMock;

    private readonly Mock<
        IUserLoginRepository>
        _userLoginRepositoryMock;

    private readonly Mock<
        IJwtTokenGenerator>
        _jwtTokenGeneratorMock;

    private readonly Mock<
        ILogger<AuthService>>
        _loggerMock;

    private readonly AuthService
        _authService;

    public AuthServiceTests()
    {
        _unitOfWorkMock =
            new Mock<IUnitOfWork>();

        _userLoginRepositoryMock =
            new Mock<
                IUserLoginRepository>();

        _jwtTokenGeneratorMock =
            new Mock<
                IJwtTokenGenerator>();

        _loggerMock =
            new Mock<
                ILogger<AuthService>>();

        _unitOfWorkMock
            .Setup(x => x.UserLogins)
            .Returns(
                _userLoginRepositoryMock
                    .Object);

        _authService =
            new AuthService(
                _unitOfWorkMock.Object,
                _jwtTokenGeneratorMock.Object,
                _loggerMock.Object);
    }

    [Fact]
    public async Task
        LoginAsync_InvalidUser_ShouldThrowException()
    {
        // Arrange

        _userLoginRepositoryMock
            .Setup(x =>
                x.FindAsync(
                    It.IsAny<
                        System.Linq.Expressions
                        .Expression<Func<
                            UserLogin,
                            bool>>>()))
            .ReturnsAsync(
                new List<UserLogin>());

        var dto =
            new LoginRequestDto
            {
                Username = "test",

                Password = "test"
            };

        // Act

        Func<Task> act = async () =>
        {
            await _authService
                .LoginAsync(dto);
        };

        // Assert

        await act.Should()
            .ThrowAsync<
                BadRequestException>();
    }
}
