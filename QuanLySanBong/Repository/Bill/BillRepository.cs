using Microsoft.EntityFrameworkCore;
using QuanLySanBong.Data;
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

        public Task UpdateAsync(BillModel bill)
        {
            _context.Bills.Update(bill);
            return _context.SaveChangesAsync();
        } 
    }
}
