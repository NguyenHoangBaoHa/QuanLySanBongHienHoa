using Microsoft.EntityFrameworkCore;
using QuanLySanBong.Data;
using QuanLySanBong.Entities.Booking.Model;

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
        public async Task<IEnumerable<BookingModel>> GetAllBookingAsync()
        {
            return await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Pitch)
                .ThenInclude(p => p.PitchType)
                .ToListAsync();
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
        public async Task<IEnumerable<BookingModel>> GetBookingsByCustomerIdAsync(int customerId)
        {
            return await _context.Bookings
                .Where(b => b.IdCustomer == customerId)
                .Include(b => b.Pitch)
                .ThenInclude(p => p.PitchType)
                .ToListAsync();
        }

        //Lấy Booking của một sân theo tuần
        public async Task<IEnumerable<BookingModel>> GetBookingsByPitchAndDateRangeAsync(int pitchId, DateTime startDate, DateTime endDate)
        {
            return await _context.Bookings
                .Where(b => b.IdPitch == pitchId && b.BookingDate >= startDate && b.BookingDate <= endDate)
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
    }
}
