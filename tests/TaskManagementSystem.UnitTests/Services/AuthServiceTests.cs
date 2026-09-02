using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using TaskManagementSystem.Application.DTOs.Auth;
using TaskManagementSystem.Application.Mappings;
using TaskManagementSystem.Application.Services;
using TaskManagementSystem.Domain.Entities;
using Xunit;

namespace TaskManagementSystem.UnitTests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly IMapper _mapper;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            var store = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(
                store.Object, null, null, null, null, null, null, null, null);

            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                roleStore.Object, null, null, null, null);

            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(x => x["JwtSettings:Secret"])
                .Returns("this-is-a-very-long-secret-key-for-testing-purposes-1234567890");
            _configurationMock.Setup(x => x["JwtSettings:Issuer"])
                .Returns("TestIssuer");
            _configurationMock.Setup(x => x["JwtSettings:Audience"])
                .Returns("TestAudience");
            _configurationMock.Setup(x => x["JwtSettings:ExpiryMinutes"])
                .Returns("60");

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();

            _authService = new AuthService(
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _configurationMock.Object,
                _mapper
            );
        }

        [Fact]
        public async global::System.Threading.Tasks.Task RegisterAsync_ShouldThrowException_WhenEmailAlreadyExists()
        {
            var registerDto = new RegisterDto
            {
                Email = "existing@example.com",
                Password = "Test123!",
                FullName = "Test User"
            };

            var existingUser = new User { Email = "existing@example.com" };
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(existingUser);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _authService.RegisterAsync(registerDto)
            );
        }

        [Fact]
        public async global::System.Threading.Tasks.Task LoginAsync_ShouldThrowException_WhenUserNotFound()
        {
            var loginDto = new LoginDto
            {
                Email = "nonexistent@example.com",
                Password = "Test123!"
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authService.LoginAsync(loginDto)
            );
        }

        [Fact]
        public async global::System.Threading.Tasks.Task LoginAsync_ShouldThrowException_WhenPasswordIsInvalid()
        {
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "WrongPassword!"
            };

            var user = new User { Id = "user123", Email = "test@example.com" };

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authService.LoginAsync(loginDto)
            );
        }
    }
}