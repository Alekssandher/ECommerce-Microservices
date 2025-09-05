using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.DTOs;
using AuthService.Models;
using BCrypt.Net;

namespace AuthService.Mappers
{
    public static class UserMapper
    {
        public static UserModel ToModel(this CreateUserDto dto, UserRoles role)
        {
            return new UserModel
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = role
            };
        }
    }
}