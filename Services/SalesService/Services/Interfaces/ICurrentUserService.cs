using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesService.Services.Interfaces
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        string Role { get; }
    }
}