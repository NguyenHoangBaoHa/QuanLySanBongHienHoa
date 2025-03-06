using QuanLySanBong.Entities.Booking.Dto;

namespace QuanLySanBong.Service.Booking
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingDto>> GetAllBookingsAsync(); //Lấy danh sách Booking theo Id (Admin, Staff)
        Task<BookingDto> GetBookingByIdAsync(int id); //Lấy Booking theo Id
        Task<IEnumerable<BookingDto>> GetBookingsByCustomerIdAsync(int customerId); //Lấy danh sách Booking của chính Customer
        Task<IEnumerable<BookingDto>> GetBookingsForPitchByWeekAsync(int pitchId, DateTime startDate); //Lấy Booking của một sân theo tuần
        Task<BookingDto> CreateBookingAsync(BookingCreateDto bookingDto); //Thêm Booking mới
        Task<bool> UpdateReceivedStatusAsync(int bookingId, bool isReceived); //Cập nhật trạng thái nhận sân của Staff
        Task<bool> DeleteBookingAsync(int id); //Xóa Booking
    }
}
