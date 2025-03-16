using QuanLySanBong.Entities.Booking.Model;

namespace QuanLySanBong.Repository.Booking
{
    public interface IBookingRepository
    {
        Task<IEnumerable<BookingModel>> GetAllBookingAsync(); //Lấy danh sách Booking theo Id (Admin, Staff)
        Task<BookingModel> GetBookingByIdAsync(int id); //Lấy Booking theo Id
        Task<IEnumerable<BookingModel>> GetBookingsByCustomerIdAsync(int customerId); //Lấy danh sách Booking của chính Customer
        Task<IEnumerable<BookingModel>> GetBookingsByPitchAndDateRangeAsync(int pitchId, DateTime startDate, DateTime endDate); //Lấy Booking của một sân theo tuần
        Task AddBookingAsync(BookingModel booking); //Thêm Booking mới
        void UpdateBooking(BookingModel booking); //Cập nhật Booking
        void DeleteBooking(BookingModel booking); //Xóa Booking
        Task<bool> IsTimeSlotAvailable(int pitchId, DateTime bookingDate, int duration); //Kiểm tra trùng Booking
    }
}
