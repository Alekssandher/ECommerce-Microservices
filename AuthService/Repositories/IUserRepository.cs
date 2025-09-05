using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Models;

namespace AuthService.Repositories
{
    public interface IUserRepository
    {
        Task<UserModel?> GetUserByEmailAsync(string email);
        Task CreateUserAsync(UserModel userModel);

    }
}