using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuanLySanBong.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace QuanLySanBong.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServiceExtensions(this IServiceCollection services, IConfiguration config)
        {
            // Kết nối cơ sở dữ liệu và bỏ qua cảnh báo Pending Model Changes
            services.AddDbContext<ApplicationDbContext>(opt =>
            {
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection"))
                   .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)); // Bỏ qua cảnh báo này
            });

            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        builder
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()
                            .SetIsOriginAllowed(hosts => true);
                    });
            });

            // SYSTEM
            services.AddSingleton(config);
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

            // AUTHENTICATION
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true, // Xác thực issuer
                        ValidateAudience = true, // Xác thực audience
                        ValidateLifetime = true, // Xác thực thời gian sống của token
                        ValidateIssuerSigningKey = true, // Xác thực khóa ký
                        ValidIssuer = config["Jwt:Issuer"], // Issuer hợp lệ
                        ValidAudience = config["Jwt:Issuer"], // Audience hợp lệ
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"])),
                        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" // Xác định nơi chứa role
                    };
                });

            return services;
        }
    }
}