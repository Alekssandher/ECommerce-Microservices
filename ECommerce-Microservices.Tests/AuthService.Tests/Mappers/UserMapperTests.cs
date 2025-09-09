using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.DTOs;
using AuthService.Mappers;
using AuthService.Models;

namespace AuthService.Tests.Mappers
{
    public sealed class UserMapperTests
    {
         [Fact]
        public void ToModel_ShouldMapFieldsCorrectly()
        {
            // Arrange
            var dto = new CreateUserDto(
                "alek",
                "alek@test.com",
                "mypassword123"
            );

            // Act
            var user = dto.ToModel(UserRoles.Client);

            // Assert
            Assert.Equal("alek", user.Username);
            Assert.Equal("alek@test.com", user.Email);
            Assert.Equal(UserRoles.Client, user.Role);
            Assert.True(BCrypt.Net.BCrypt.Verify("mypassword123", user.Password));
        }

        [Fact]
        public void ToModel_ShouldAssignGivenRole()
        {
            // Arrange
            var dto = new CreateUserDto(
                "manageruser",
                "manager@test.com",
                "securepass"
            );

            // Act
            var user = dto.ToModel(UserRoles.Manager);

            // Assert
            Assert.Equal(UserRoles.Manager, user.Role);
            Assert.True(BCrypt.Net.BCrypt.Verify("securepass", user.Password));
        }

        [Fact]
        public void ToModel_ShouldGenerateUniqueHashForSamePassword()
        {
            // Arrange
            var dto1 = new CreateUserDto("user1", "u1@test.com", "samepass");
            var dto2 = new CreateUserDto("user2", "u2@test.com", "samepass");

            // Act
            var user1 = dto1.ToModel(UserRoles.Client);
            var user2 = dto2.ToModel(UserRoles.Client);

            // Assert
            Assert.NotEqual(user1.Password, user2.Password); // BCrypt gera salt, hashes diferentes
            Assert.True(BCrypt.Net.BCrypt.Verify("samepass", user1.Password));
            Assert.True(BCrypt.Net.BCrypt.Verify("samepass", user2.Password));
        }
    }
}