using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Shared.Extensions
{
    public static class JwtExtensions
    {
        public static void RegisterAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            var (jwtKey, jwtIssuer, jwtAudience) = (
                configuration["JwtSettings:Key"]
                    ?? throw new Exception("Jwt key is missing in AppSettings"),
                configuration["JwtSettings:Issuer"]
                    ?? throw new Exception("Jwt issuer is missing in AppSettings"),
                configuration["JwtSettings:Audience"]
                    ?? throw new Exception("Jwt audience is missing in AppSettings")
            );

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        RoleClaimType = "Role"
                    };
                });

            services.AddHttpContextAccessor();
            services.AddAuthentication();
            services.AddAuthorization();
        }
    }
}