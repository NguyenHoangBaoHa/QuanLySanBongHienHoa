using QuanLySanBong.Entities.Account.Dto;
using QuanLySanBong.Entities.Account.Model;
using QuanLySanBong.Entities.Customer.Dto;
using QuanLySanBong.Entities.Customer.Model;
using QuanLySanBong.Entities.Staff.Model;
using QuanLySanBong.Helpers;
using QuanLySanBong.UnitOfWork;

namespace QuanLySanBong.Service.Account
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly ITokenService _tokenService;

        public AccountService(IUnitOfWork unitOfWork, IConfiguration config, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _tokenService = tokenService;
        }

        public async Task<LoginModel> Login(LoginDto loginDto)
        {
            var acc = await _unitOfWork.Accounts.GetByEmail(loginDto.Email);
            if(acc == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, acc.Password))
            {
                return null;
            }

            var token = _tokenService.CreateToken(acc, _config);

            return new LoginModel()
            {
                Username = acc.Email,
                Token = token,
                Role = acc.Role,
            };
        }

        private async Task<bool> IsEmailExists(string email)
        {
            return await _unitOfWork.Accounts.GetByEmail(email) != null;
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Tạo tài khoản Staff (chỉ Admin mới làm được)
        public async Task<AccountModel> CreateStaff(CreateStaffDto createStaffDto)
        {
            if (await IsEmailExists(createStaffDto.Email))
            {
                throw new InvalidOperationException("Email đã tồn tại.");
            }

            var staff = new StaffModel
            {
                DisplayName = createStaffDto.DisplayName,
                DateOfBirth = createStaffDto.DateOfBirth,
                CCCD = createStaffDto.CCCD,
                Gender = createStaffDto.Gender,
                PhoneNumber = createStaffDto.PhoneNumber,
                Address = createStaffDto.Address,
                StartDate = createStaffDto.StartDate,
            };

            var account = new AccountModel
            {
                Email = createStaffDto.Email,
                Password = HashPassword(createStaffDto.Password),
                Role = "Staff",
                Staff = staff
            };

            await _unitOfWork.Staffs.Add(staff);
            await _unitOfWork.Accounts.Add(account);
            await _unitOfWork.CompleteAsync();

            return account;
        }

        public async Task<AccountModel> RegisterCustomer(CustomerDto customerDto, AccountDto accountDto)
        {
            var existingAccount = await _unitOfWork.Accounts.GetByEmail(accountDto.Email);
            if (existingAccount != null)
            {
                throw new InvalidOperationException("Email đã tồn tại.");
            }

            var customer = new CustomerModel
            {
                DisplayName = customerDto.DisplayName,
                DateOfBirth = customerDto.DateOfBirth,
                CCCD = customerDto.CCCD,
                Gender = customerDto.Gender,
                PhoneNumber = customerDto.PhoneNumber,
                Address = customerDto.Address,
            };

            var account = new AccountModel
            {
                Email = accountDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(accountDto.Password), // Hash mật khẩu
                Role = "Customer",
                IdCustomer = customer.Id,
            };

            try
            {
                // Lưu khách hàng trước để lấy ID
                await _unitOfWork.Accounts.AddCustomer(customer);
                await _unitOfWork.CompleteAsync();

                if (customer.Id == 0)
                {
                    throw new InvalidOperationException("Không thể lấy ID của khách hàng sau khi lưu.");
                }

                // Liên kết ID khách hàng với tài khoản
                account.IdCustomer = customer.Id;

                // Lưu tài khoản
                await _unitOfWork.Accounts.Add(account);
                await _unitOfWork.CompleteAsync();

                return account;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi đăng ký tài khoản: {ex.Message}");
            }
        }
    }
}
