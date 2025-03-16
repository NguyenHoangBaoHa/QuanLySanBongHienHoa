using Microsoft.EntityFrameworkCore;
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
                        builder.WithOrigins("http://localhost:3000")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
            });

            // SYSTEM
            services.AddSingleton(config);
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

            // AUTHENTICATION
            // Đọc thời gian sống của token từ `appsettings.json`
            var tokenLifetime = config.GetValue<int>("Jwt:TokenLifetime");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = config["Jwt:Issuer"],
                            ValidAudience = config["Jwt:Issuer"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"])),
                            ClockSkew = TimeSpan.Zero,
                            LifetimeValidator = (before, expires, token, parameters) =>
                            {
                                return expires > DateTime.UtcNow.AddSeconds(tokenLifetime); // Kiểm tra xem token có còn hợp lệ không
                            }
                        };
                    });

            return services;
        }
    }
}