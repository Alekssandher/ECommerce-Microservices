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
using Serilog;
using Shared.Exceptions;

namespace AuthService.Services
{
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
            Log.Information("Login attempt for email: {Email}", loginDto.Email);
            
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);

            if (user is null)
            {
                Log.Warning("Failed login attempt for non-existent email: {Email}", loginDto.Email);
                throw new Exceptions.BadRequestException("Wrong User or Password.");
            }
            

            var validPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password);

            if (validPassword == false)
            {
                Log.Warning("Failed login attempt for email: {Email} - Invalid password", loginDto.Email);
                throw new Exceptions.BadRequestException("Wrong User or Password.");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("Role", user.Role.ToString()),
                new Claim("UserId", user.Id.ToString())
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

            Log.Information("Generated Token For User ID: {UserId}, Email: {Email}, Role: {Role} - Expires: {Expires}",
                user.Id,
                user.Email,
                user.Role,
                token.ValidTo.ToLocalTime());

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
            Log.Information("Checking if user exists for email: {Email}", email);

            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user is not null)
            {
                Log.Warning("User already exists attempt for email: {Email}", email);
                throw new Exceptions.BadRequestException("User Already Exists With This Email.");
            }
                

            return;
            
        }
        public async Task CreateUser(CreateUserDto createUserDto)
        {
            Log.Information("Creating client user for email: {Email}", createUserDto.Email);

            await CheckIfUserExists(createUserDto.Email);
            await _userRepository.CreateUserAsync(createUserDto.ToModel(UserRoles.Client));
        }

        public async Task CreateManager(CreateUserDto createUserDto)
        {
            Log.Information("Creating manager user for email: {Email}", createUserDto.Email);

            await CheckIfUserExists(createUserDto.Email);
            await _userRepository.CreateUserAsync(createUserDto.ToModel(UserRoles.Manager));
        }
    }
}