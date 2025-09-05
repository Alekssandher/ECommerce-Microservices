using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.DTOs;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.ModelViews;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;

        public AuthController(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _jwtService.GenerateJwtToken(dto);

            return Ok(new OkResponse<string>(string.Empty, string.Empty, token));
        }

        [HttpPost("register/user")]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserDto dto)
        {
            await _jwtService.CreateUser(dto);

            return Created();
        }

        [HttpPost("register/manager")]
        public async Task<IActionResult> RegisterManager([FromBody] CreateUserDto dto)
        {
            await _jwtService.CreateManager(dto);

            return Created();
        }
    }
}