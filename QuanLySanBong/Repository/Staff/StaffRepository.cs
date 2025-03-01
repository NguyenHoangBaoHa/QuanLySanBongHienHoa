using QuanLySanBong.Data;
using QuanLySanBong.Entities.Staff.Model;

namespace QuanLySanBong.Repository.Staff
{
    public class StaffRepository : IStaffRepository
    {
        private readonly ApplicationDbContext _context;

        public StaffRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<StaffModel> GetStaffByAccountId(int accountId)
        {
            // Tìm tài khoản dựa trên accountId
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null || account.IdStaff == null)
            {
                return null; // Trả về null nếu không tìm thấy tài khoản hoặc IdStaff là null
            }

            // Tìm nhân viên dựa trên IdStaff từ tài khoản
            return await _context.Staffs.FindAsync(account.IdStaff);
        }

        public async Task Add(StaffModel staff)
        {
            await _context.Staffs.AddAsync(staff);
        }

        public async Task Update(StaffModel staff)
        {
            // Cập nhật thông tin nhân viên
            _context.Staffs.Update(staff);
            await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
        }
    }
}
