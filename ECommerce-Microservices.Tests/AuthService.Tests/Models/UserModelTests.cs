using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Models;

namespace AuthService.Tests.Models
{
    public sealed class UserModelTests
    {
        [Fact]
        public void CanSetAndGetProperties()
        {
            // Arrange
            var user = new UserModel
            {
                Id = 1,
                Username = "alex",
                Email = "alex@test.com",
                Password = "hashed-password",
                Role = UserRoles.Manager
            };

            // Assert
            Assert.Equal(1, user.Id);
            Assert.Equal("alex", user.Username);
            Assert.Equal("alex@test.com", user.Email);
            Assert.Equal("hashed-password", user.Password);
            Assert.Equal(UserRoles.Manager, user.Role);
        }

        [Fact]
        public void DefaultValues_ShouldBeInitialized()
        {
            // Arrange
            var user = new UserModel();

            // Assert
            Assert.Equal(0, user.Id);                     
            Assert.Null(user.Username);                
            Assert.Null(user.Email);
            Assert.Null(user.Password);
        
            Assert.Equal(UserRoles.Client, user.Role);    
        }

        [Theory]
        [InlineData(UserRoles.Client)]
        [InlineData(UserRoles.Manager)]
        public void CanAssignDifferentRoles(UserRoles role)
        {
            // Arrange
            var user = new UserModel { Role = role };

            // Assert
            Assert.Equal(role, user.Role);
        }
    }
}