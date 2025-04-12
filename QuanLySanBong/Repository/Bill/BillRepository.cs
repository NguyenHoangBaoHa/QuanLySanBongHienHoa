using Microsoft.EntityFrameworkCore;
using QuanLySanBong.Data;
using QuanLySanBong.Entities.Bill.Dto;
using QuanLySanBong.Entities.Bill.Model;

namespace QuanLySanBong.Repository.Bill
{
    public class BillRepository : IBillRepository
    {
        private readonly ApplicationDbContext _context;
        public BillRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<BillModel>> GetAllAsync(int page, int pageSize)
        {
            return await _context.Bills
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(b => b.Booking)
                .Include(b => b.PaidBy)
                .ToListAsync();
        }

        public async Task<BillModel> GetByIdAsync(int id)
        {
            return await _context.Bills
                .Include(b => b.Booking)
                .Include(b => b.PaidBy)
                .FirstOrDefaultAsync(b => b.Id == id);
        }
        public async Task<BillModel> CreateAsync(BillModel bill)
        {
            await _context.Bills.AddAsync(bill);
            await _context.SaveChangesAsync();
            return bill;
        }

        public Task UpdateAsync(BillModel bill)
        {
            _context.Bills.Update(bill);
            return _context.SaveChangesAsync();
        }

        public IQueryable<BillModel> GetQueryable()
        {
            return _context.Bills.AsQueryable();
        }

        public async Task<List<BillModel>> GetBillsByCustomerIdAsync(int customerId)
        {
            return await _context.Bills
                .Where(b => b.Booking.IdCustomer == customerId)
                .Include(b => b.Booking)
                .ToListAsync();
        }

        public async Task<BillModel?> GetBillByBookingIdAsync(int bookingId)
        {
            return await _context.Bills
                .Include(b => b.Booking)
                    .ThenInclude(bk => bk.Customer)
                .Include(b => b.Booking.Pitch)
                    .ThenInclude(p => p.PitchType)
                .FirstOrDefaultAsync(b => b.IdBooking == bookingId);
        }
    }
}
