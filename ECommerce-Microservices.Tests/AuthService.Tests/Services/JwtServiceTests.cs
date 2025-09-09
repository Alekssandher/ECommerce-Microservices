using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.DTOs;
using AuthService.Models;
using AuthService.Repositories;
using AuthService.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Shared.Exceptions;

namespace AuthService.Tests.Services
{
    public class JwtServiceTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly JwtService _jwtService;

        private readonly string _jwtKey = "SuperSecretKeyForTestingOnly123!";
        private readonly string _jwtIssuer = "TestIssuer";
        private readonly string _jwtAudience = "TestAudience";

        public JwtServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _userRepositoryMock = new Mock<IUserRepository>();

            _configurationMock.Setup(c => c["JwtSettings:Key"]).Returns(_jwtKey);
            _configurationMock.Setup(c => c["JwtSettings:Issuer"]).Returns(_jwtIssuer);
            _configurationMock.Setup(c => c["JwtSettings:Audience"]).Returns(_jwtAudience);

            _jwtService = new JwtService(_configurationMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public async Task GenerateJwtToken_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var loginDto = new LoginDto ( "user@test.com","123456");

            var user = new UserModel
            {
                Id = 1,
                Email = "user@test.com",
                Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                Role = UserRoles.Client
            };

            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(loginDto.Email))
                            .ReturnsAsync(user);

            // Act
            var token = await _jwtService.GenerateJwtToken(loginDto);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(token));
            Assert.Contains(".", token); 
        }

        [Fact]
        public async Task GenerateJwtToken_ShouldThrow_WhenUserNotFound()
        {
            // Arrange
            var loginDto = new LoginDto ("notfound@test.com", "123456");
            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(loginDto.Email))
                            .ReturnsAsync((UserModel?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exceptions.BadRequestException>(() => _jwtService.GenerateJwtToken(loginDto));
        }

        [Fact]
        public async Task GenerateJwtToken_ShouldThrow_WhenPasswordIsInvalid()
        {
            // Arrange
            var loginDto = new LoginDto ("user@test.com", "wrongpass" );
            var user = new UserModel
            {
                Id = 1,
                Email = "user@test.com",
                Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                Role = UserRoles.Client
            };

            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(loginDto.Email))
                            .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<Exceptions.BadRequestException>(() => _jwtService.GenerateJwtToken(loginDto));
        }

        [Fact]
        public async Task ExtractRole_ShouldReturnRoleFromToken()
        {
            // Arrange
            var loginDto = new LoginDto ("user@test.com", "123456" );
            var user = new UserModel
            {
                Id = 1,
                Email = "user@test.com",
                Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                Role = UserRoles.Manager
            };

            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(loginDto.Email))
                            .ReturnsAsync(user);

            var token = await _jwtService.GenerateJwtToken(loginDto);

            // Act
            var role = _jwtService.ExtractRole(token);

            // Assert
            Assert.Equal(UserRoles.Manager.ToString(), role);
        }

        [Fact]
        public async Task ExtractEmail_ShouldReturnEmailFromToken()
        {
            // Arrange
            var loginDto = new LoginDto ("user@test.com",  "123456" );
            var user = new UserModel
            {
                Id = 1,
                Email = "user@test.com",
                Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                Role = UserRoles.Client
            };

            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(loginDto.Email))
                            .ReturnsAsync(user);

            var token = await _jwtService.GenerateJwtToken(loginDto);

            // Act
            var email = _jwtService.ExtractEmail(token);

            // Assert
            Assert.Equal("user@test.com", email);
        }

        [Fact]
        public async Task CreateUser_ShouldCallRepository_WhenUserDoesNotExist()
        {
            // Arrange
            var dto = new CreateUserDto ("username", "new@test.com", "123" );
            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(dto.Email))
                            .ReturnsAsync((UserModel?)null);

            // Act
            await _jwtService.CreateUser(dto);

            // Assert
            _userRepositoryMock.Verify(r => r.CreateUserAsync(It.IsAny<UserModel>()), Times.Once);
        }

        [Fact]
        public async Task CreateUser_ShouldThrow_WhenUserAlreadyExists()
        {
            // Arrange
            var dto = new CreateUserDto ("username", "exists@test.com", "123" );
            var user = new UserModel { Id = 1, Email = dto.Email, Password = "hash", Role = UserRoles.Client };

            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(dto.Email))
                            .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<Exceptions.BadRequestException>(() => _jwtService.CreateUser(dto));
        }
    }

}