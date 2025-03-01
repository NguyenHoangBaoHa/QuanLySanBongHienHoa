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

        public async Task<List<BookingModel>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Pitch)
                .ThenInclude(p => p.PitchType)
                .ToListAsync();
        }

        public async Task<BookingModel> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Pitch)
                .ThenInclude(p => p.PitchType)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task UpdateReceivedStatusAsync(int id, bool isReceived)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                throw new KeyNotFoundException("Booking not found.");
            }

            booking.IsReceived = isReceived;
            booking.UpdateAt = DateTime.UtcNow;

            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }
    }
}
