using QuanLySanBong.Entities.Staff.Model;

namespace QuanLySanBong.Repository.Staff
{
    public interface IStaffRepository
    {
        Task<StaffModel> GetStaffByAccountId(int accountId);
        Task Add(StaffModel staff);
        Task Update(StaffModel staff);
    }
}
