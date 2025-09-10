using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AuthService.Infraestructure.Data
{
    public static class SeedData
    {
        public static async Task EnsureTestUserAsync(UsersContext db)
        {
            var existingUser = await db.Users
                .FirstOrDefaultAsync(u => u.Email == "manager@test.com");

            if (existingUser != null)
                return;

            var user = new UserModel
            {
                Username = "Manager Test",
                Email = "manager@test.com",
                Password = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = UserRoles.Manager,
                CreatedAt = DateTime.UtcNow
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();
            Log.Information("Manager User Test Created.");
        }
    }
}