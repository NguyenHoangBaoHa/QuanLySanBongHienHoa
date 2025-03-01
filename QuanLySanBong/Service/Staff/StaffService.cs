using QuanLySanBong.Entities.Staff.Dto;
using QuanLySanBong.UnitOfWork;

namespace QuanLySanBong.Service.Staff
{
    public class StaffService : IStaffService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StaffService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<StaffDto> GetPersonalInfo(int accountId)
        {
            // Lấy thông tin nhân viên dựa trên accountId
            var staff = await _unitOfWork.Staffs.GetStaffByAccountId(accountId);

            if (staff == null)
                return null; // Trả về null nếu không tìm thấy nhân viên

            // Chuyển đổi từ StaffModel sang StaffDto
            return new StaffDto
            {
                DisplayName = staff.DisplayName,
                DateOfBirth = staff.DateOfBirth,
                CCCD = staff.CCCD,
                Gender = staff.Gender,
                PhoneNumber = staff.PhoneNumber,
                Address = staff.Address,
                StartDate = staff.StartDate
            };
        }

        public async Task<bool> UpdatePersonalInfo(int accountId, StaffDto staffDto)
        {
            // Lấy thông tin nhân viên hiện tại
            var staff = await _unitOfWork.Staffs.GetStaffByAccountId(accountId);

            if (staff == null)
                return false; // Trả về false nếu không tìm thấy nhân viên

            // Cập nhật thông tin cho nhân viên
            staff.DisplayName = staffDto.DisplayName;
            staff.DateOfBirth = staffDto.DateOfBirth;
            staff.CCCD = staffDto.CCCD;
            staff.Gender = staffDto.Gender;
            staff.PhoneNumber = staffDto.PhoneNumber;
            staff.Address = staffDto.Address;
            staff.StartDate = staffDto.StartDate;

            // Gọi hàm cập nhật trong repository
            await _unitOfWork.Staffs.Update(staff);
            return true; // Trả về true nếu cập nhật thành công
        }
    }
}
