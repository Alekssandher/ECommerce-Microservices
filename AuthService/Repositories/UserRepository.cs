using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Infraestructure.Data;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AuthService.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UsersContext _usersContext;

        public UserRepository(UsersContext usersContext)
        {
            _usersContext = usersContext;
        }

        public async Task CreateUserAsync(UserModel userModel)
        {
            await _usersContext.Users.AddAsync(userModel);
            await _usersContext.SaveChangesAsync();

            Log.Information($"Registred user with ID: {userModel.Id}");
        }

        public async Task<UserModel?> GetUserByEmailAsync(string email)
        {
            return await _usersContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}