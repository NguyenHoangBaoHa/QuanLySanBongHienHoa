using QuanLySanBong.Entities.Customer.Dto;

namespace QuanLySanBong.Entities.Account.Dto
{
    public class RegisterCustomerDto
    {
        public AccountDto Account { get; set; }
        public CustomerDto Customer { get; set; }
    }
}
