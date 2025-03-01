using QuanLySanBong.Entities.Account.Dto;
using QuanLySanBong.Entities.Account.Model;
using QuanLySanBong.Entities.Customer.Dto;

namespace QuanLySanBong.Service.Account
{
    public interface IAccountService
    {
        Task<LoginModel> Login(LoginDto loginDto);
        Task<AccountModel> CreateStaff(CreateStaffDto createStaffDto);
        Task<AccountModel> RegisterCustomer(CustomerDto customerDto, AccountDto accountDto);
    }
}
