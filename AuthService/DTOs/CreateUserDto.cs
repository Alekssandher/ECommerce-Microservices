using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.DTOs
{
    public record CreateUserDto(
        string Username,
        string Email,
        string Password
    );
}