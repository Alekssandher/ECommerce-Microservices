using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Models
{
    public class UserModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; init; }
        public string Username { get; init; } = default!;
        public string Email { get; init; } = default!;
        public string Password { get; init; } = default!;
        public UserRoles Role { get; init; }
    }

    public enum UserRoles
    {
        Client,
        Manager
    }
}
