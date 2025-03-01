using QuanLySanBong.Entities.Account.Model;

namespace QuanLySanBong.Helpers
{
    public interface ITokenService
    {
        string CreateToken(AccountModel acc, IConfiguration config);
    }
}
