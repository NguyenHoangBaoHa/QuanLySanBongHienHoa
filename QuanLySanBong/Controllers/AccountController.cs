using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLySanBong.Entities.Account.Dto;
using QuanLySanBong.Service.Account;

namespace QuanLySanBong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _service;

        public AccountController(IAccountService service)
        {
            _service = service;
        }

        // Đăng nhập
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            var loginModel = await _service.Login(loginDto);

            if (string.IsNullOrEmpty(loginModel.Token))
                return Unauthorized("Sai email hoặc mật khẩu.");

            return Ok(new
            {
                Username = loginModel.Username,
                Token = loginModel.Token,
                Role = loginModel.Role
            });
        }

        [HttpPost("create-staff")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffDto createStaffDto)
        {
            if (createStaffDto == null)
            {
                return BadRequest("Invalid data.");
            }

            // Lấy claims của user đang đăng nhập
            var userClaims = HttpContext.User.Claims;

            // Kiểm tra xem người dùng có quyền admin không
            var roleClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
            if (roleClaim != "Admin")
            {
                return Unauthorized("You do not have permission to perform this action.");
            }

            try
            {
                // Gọi dịch vụ để tạo tài khoản nhân viên
                var account = await _service.CreateStaff(createStaffDto);

                // Trả về kết quả thành công
                return Ok(new
                {
                    Message = "Staff created successfully.",
                    AccountId = account.Id,
                    Email = account.Email,
                    Role = account.Role
                });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error saving staff or account: {ex.Message}");
            }
        }

        [HttpPost("register-customer")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterCustomerDto registerCustomerDto)
        {
            if(registerCustomerDto == null || registerCustomerDto.Account == null || registerCustomerDto.Customer == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            try
            {
                var account = await _service.RegisterCustomer(registerCustomerDto.Customer, registerCustomerDto.Account);

                return Ok(new
                {
                    message = "Đăng ký tài khoản thành công.",
                    email = account.Email,
                    role = account.Role
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Lỗi hệ thống: {ex.Message}" });
            }
        }
    }
}
