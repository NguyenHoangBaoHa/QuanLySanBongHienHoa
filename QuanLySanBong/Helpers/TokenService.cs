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
            // Lấy thời gian sống token từ cấu hình
            var tokenLifetime = int.Parse(config["Jwt:TokenLifetime"] ?? "60"); //Mặc định là 60 phút

            //Danh sách claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, account.Email),
                new Claim(ClaimTypes.Role, account.Role), // Lấy vai trò của người dùng
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
                expires: DateTime.UtcNow.AddMinutes(tokenLifetime),
                signingCredentials: creds
            );

            // Trả token về dạng chuỗi
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
