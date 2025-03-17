using Microsoft.EntityFrameworkCore;
using QuanLySanBong.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace QuanLySanBong.Extensions
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using QuanLySanBong.Entities.Account.Model;

    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServiceExtensions(this IServiceCollection services, IConfiguration config)
        {
            // Kết nối cơ sở dữ liệu, bỏ qua cảnh báo Pending Model Changes
            services.AddDbContext<ApplicationDbContext>(opt =>
            {
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection"))
                   .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
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

            // SYSTEM CONFIG
            services.AddSingleton(config);
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

            // 🔹 Lấy giá trị JWT từ cấu hình
            var jwtKey = config["Jwt:Key"];
            var jwtIssuer = config["Jwt:Issuer"];
            var jwtLifetime = config.GetValue<int>("Jwt:TokenLifetime");

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer))
            {
                throw new InvalidOperationException("JWT configuration is missing in appsettings.json.");
            }

            // AUTHENTICATION
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
                        ValidAudience = jwtIssuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        ClockSkew = TimeSpan.Zero // 🔹 Loại bỏ trễ thời gian xác thực token
                    };

                    // 📌 Xử lý sự kiện xác thực token
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                            if (claimsIdentity != null)
                            {
                                Console.WriteLine("✅ Token validated. Claims:");
                                foreach (var claim in claimsIdentity.Claims)
                                {
                                    Console.WriteLine($"🔹 {claim.Type}: {claim.Value}");
                                }
                            }
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine($"❌ Lỗi xác thực token: {context.Exception.Message}");
                            return Task.CompletedTask;
                        }
                    };
                });

            return services;
        }

        // ✅ Hàm tạo token trong hệ thống (Cải tiến)
        public static string GenerateJwtToken(AccountModel account, IConfiguration config)
        {
            var jwtKey = config["Jwt:Key"];
            var jwtIssuer = config["Jwt:Issuer"];
            var jwtLifetime = config.GetValue<int>("Jwt:TokenLifetime");

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer))
            {
                throw new InvalidOperationException("JWT configuration is missing in appsettings.json.");
            }

            // 📌 Claims đảm bảo chứa đủ IdCustomer và Role
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
                new Claim("IdCustomer", account.IdCustomer?.ToString() ?? "0"), // 🔹 Đảm bảo luôn có giá trị
                new Claim(ClaimTypes.Role, account.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtIssuer,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtLifetime),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
