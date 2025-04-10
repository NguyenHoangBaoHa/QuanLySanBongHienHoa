using Microsoft.EntityFrameworkCore;
using QuanLySanBong.Data;
using QuanLySanBong.Entities.Booking.Dto;
using QuanLySanBong.Entities.Booking.Model;
using QuanLySanBong.Entities.Enums;

namespace QuanLySanBong.Repository.Booking
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        //Lấy danh sách Booking theo Id (Admin, Staff)
        public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Customer) // 🛠 Gắn bảng Customer để lấy số điện thoại
                .Include(b => b.Pitch)
                .ThenInclude(p => p.PitchType)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    IdPitch = b.Pitch.Id,
                    DisplayName = b.Customer.DisplayName,  // 🛠 Lấy họ tên từ Customer
                    PhoneNumber = b.Customer.PhoneNumber, // 🛠 Thêm SĐT từ Customer
                    PitchName = b.Pitch.Name,
                    PitchTypeName = b.Pitch.PitchType.Name,
                    BookingDate = b.BookingDate,
                    Duration = b.Duration,
                    PaymentStatus = b.PaymentStatus,
                    TimeslotStatus = b.TimeslotStatus,
                    IsReceived = b.IsReceived
                })
                .ToListAsync();

            return bookings;
        }

        //Lấy Booking theo Id
        public async Task<BookingModel> GetBookingByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Pitch)
                .ThenInclude(p => p.PitchType)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        //Lấy danh sách Booking của chính Customer
        public async Task<List<BookingDto>> GetBookingsByCustomerIdAsync(int customerId)
        {
            return await _context.Bookings
                .Where(b => b.IdCustomer == customerId)
                .Include(b => b.Pitch)
                .ThenInclude(p => p.PitchType)
                .OrderByDescending(b => b.BookingDate) // 🛠 Sắp xếp giảm dần
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    IdCustomer = b.IdCustomer,
                    IdPitch = b.Pitch.Id,
                    PitchName = b.Pitch.Name,
                    PitchTypeName = b.Pitch.PitchType.Name,
                    BookingDate = b.BookingDate,
                    Duration = b.Duration,
                    PaymentStatus = b.PaymentStatus,
                    TimeslotStatus = b.TimeslotStatus,
                    IsReceived = b.IsReceived
                })
                .ToListAsync();
        }

        //Lấy Booking của một sân theo tuần
        public async Task<IEnumerable<BookingModel>> GetBookingsByPitchAndDateRangeAsync(int pitchId, DateTime startDate, DateTime endDate)
        {
            return await _context.Bookings
                .Where(b => b.IdPitch == pitchId &&
                            b.BookingDate >= startDate &&
                            b.BookingDate <= endDate &&
                            !b.IsCanceled)
                .Include(b => b.Customer)
                .Include(b => b.Pitch)
                .ThenInclude(p => p.PitchType)
                .ToListAsync();
        }

        //Thêm Booking mới
        public async Task AddBookingAsync(BookingModel booking)
        {
            await _context.Bookings.AddAsync(booking);
        }

        //Cập nhật Booking
        public void UpdateBooking(BookingModel booking)
        {
            _context.Bookings.Update(booking);
        }

        //Xóa Booking
        public void DeleteBooking(BookingModel booking)
        {
            _context.Bookings.Remove(booking);
        }

        public async Task<bool> CancelBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
            if(booking == null || booking.IsCanceled)
            {
                return false;
            }

            DateTime now = DateTime.UtcNow;
            if(booking.BookingDate <= now.AddHours(1))
            {
                return false;
            }

            booking.IsCanceled = true;
            booking.UpdateTimestamp();

            _context.Bookings.Update(booking);
            return true;
        }

        // Kiểm tra khung giờ có trùng không
        public async Task<bool> IsTimeSlotAvailable(int pitchId, DateTime bookingDate, int duration)
        {
            DateTime bookingEndTime = bookingDate.AddMinutes(duration);

            var isBooked = await _context.Bookings
                .Where(b => b.IdPitch == pitchId &&
                            b.BookingDate.Date == bookingDate.Date && // ✅ Sửa ở đây
                            b.BookingDate.AddMinutes(b.Duration) > bookingDate &&
                            b.BookingDate < bookingEndTime &&
                            b.TimeslotStatus == TimeslotStatus.Booked)
                .FirstOrDefaultAsync();

            return isBooked == null; // ✅ Trả về true nếu KHÔNG có bản ghi trùng
        }
    }
}
