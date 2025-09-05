using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.DTOs;

namespace AuthService.Services
{
    public interface IJwtService
    {
        Task<string> GenerateJwtToken(LoginDto loginDto);
        Task CreateUser(CreateUserDto createUserDto);
        Task CreateManager(CreateUserDto createUserDto);
        string ExtractEmail(string token);
        string ExtractRole(string token);
    }
}