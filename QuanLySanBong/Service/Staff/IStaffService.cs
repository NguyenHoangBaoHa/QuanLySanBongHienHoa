using QuanLySanBong.Entities.Staff.Dto;

namespace QuanLySanBong.Service.Staff
{
    public interface IStaffService
    {
        Task<StaffDto> GetPersonalInfo(int accountId);
        Task<bool> UpdatePersonalInfo(int accountId, StaffDto staffDto);
    }
}
