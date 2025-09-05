using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthService.DTOs;
using AuthService.Mappers;
using AuthService.Models;
using AuthService.Repositories;
using Microsoft.IdentityModel.Tokens;
using Shared.Exceptions;

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

    public class JwtService : IJwtService
    {
        private readonly string jwtKey;
        private readonly string jwtIssuer;
        private readonly string jwtAudience;

        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        public JwtService(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;

            (jwtKey, jwtIssuer, jwtAudience) = (
                _configuration["JwtSettings:Key"]
                    ?? throw new Exception("Jwt key is missing in AppSettings"),
                _configuration["JwtSettings:Issuer"]
                    ?? throw new Exception("Jwt issuer is missing in AppSettings"),
                _configuration["JwtSettings:Audience"]
                    ?? throw new Exception("Jwt audience is missing in AppSettings")
            );

            _userRepository = userRepository;
        }
        
        public async Task<string> GenerateJwtToken(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email)
                ?? throw new Exceptions.BadRequestException("Wrong User or Password.");

            var hash = BCrypt.Net.BCrypt.HashPassword(loginDto.Password);

            if (user.Password != hash)
            {
                throw new Exceptions.BadRequestException("Wrong User or Password.");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("Role", user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private ClaimsPrincipal ExtractPrincipal(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true
            };

            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }
        public string ExtractRole(string token)
        {
            try
            {
                var principal = ExtractPrincipal(token);

                var emailClaim = principal.Claims
                    .FirstOrDefault(c =>
                        c.Type == "Role" ||
                        c.Type == "role" 
                    );


                return emailClaim?.Value ?? throw new Exception("Role claim not found in token");
            }
            catch (SecurityTokenExpiredException)
            {
                throw new Exception("Token has expired");
            }
            catch (SecurityTokenException ex)
            {
                throw new Exception($"Invalid token: {ex.Message}");
            }
        }
        public string ExtractEmail(string token)
        {


            try
            {

                var principal = ExtractPrincipal(token);

                var emailClaim = principal.Claims
                    .FirstOrDefault(c =>
                        c.Type == JwtRegisteredClaimNames.Email ||
                        c.Type == "email" ||
                        c.Type == ClaimTypes.Email
                    );


                return emailClaim?.Value ?? throw new Exception("Email claim not found in token");
            }
            catch (SecurityTokenExpiredException)
            {
                throw new Exception("Token has expired");
            }
            catch (SecurityTokenException ex)
            {
                throw new Exception($"Invalid token: {ex.Message}");
            }
        }

        private async Task CheckIfUserExists(string email)
        {
            _ = await _userRepository.GetUserByEmailAsync(email)
                ?? throw new Exceptions.BadRequestException("User Already Exists With This Email.");
        }
        public async Task CreateUser(CreateUserDto createUserDto)
        {
            await CheckIfUserExists(createUserDto.Email);
            await _userRepository.CreateUserAsync(createUserDto.ToModel(UserRoles.Client));
        }

        public async Task CreateManager(CreateUserDto createUserDto)
        {
            await CheckIfUserExists(createUserDto.Email);
            await _userRepository.CreateUserAsync(createUserDto.ToModel(UserRoles.Manager));
        }
    }
}