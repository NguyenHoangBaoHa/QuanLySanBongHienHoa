using QuanLySanBong.Entities.Account.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace QuanLySanBong.Helpers
{
    public class TokenService : ITokenService
    {
        public string CreateToken(AccountModel account, IConfiguration config)
        {
            // Lấy thời gian sống token từ cấu hình, đảm bảo giá trị hợp lệ
            if (!int.TryParse(config["Jwt:TokenLifetime"], out int tokenLifetime))
            {
                tokenLifetime = 60; // Nếu không đọc được từ config, mặc định 60 phút
            }

            // Danh sách claims
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, account.Email),
            new Claim(ClaimTypes.Role, account.Role),
            new Claim("DisplayName", account.Customer?.DisplayName ?? account.Staff?.DisplayName ?? "Unknown"),
        };

            // Khóa bảo mật
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Tạo token
            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(tokenLifetime), // ✅ Đảm bảo Token có thời gian hợp lệ
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
    
}
