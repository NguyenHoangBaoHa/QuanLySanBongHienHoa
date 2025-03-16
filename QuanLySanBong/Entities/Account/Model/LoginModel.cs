namespace QuanLySanBong.Entities.Account.Model
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public int? IdCustomer { get; set; }  // Thêm IdCustomer
    }
}
