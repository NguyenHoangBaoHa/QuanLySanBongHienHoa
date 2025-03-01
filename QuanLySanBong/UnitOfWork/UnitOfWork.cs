using Microsoft.EntityFrameworkCore;
using QuanLySanBong.Data;
using QuanLySanBong.Repository.Account;
using QuanLySanBong.Repository.Booking;
using QuanLySanBong.Repository.Pitch;
using QuanLySanBong.Repository.PitchType;
using QuanLySanBong.Repository.Staff;

namespace QuanLySanBong.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            Accounts = new AccountRepository(_context);

            PitchTypeImages = new PitchTypeImageRepository(_context);
            
            PitchTypes = new PitchTypeRepository(_context);

            Pitches = new PitchRepository(_context);

            Bookings = new BookingRepository(_context);

            Staffs = new StaffRepository(_context);

        }

        public IAccountRepository Accounts { get; private set; }

        public IPitchTypeImageRepository PitchTypeImages { get; private set; }

        public IPitchTypeRepository PitchTypes { get; private set; }

        public IPitchRepository Pitches { get; private set; }

        public IBookingRepository Bookings { get; private set; }

        public IStaffRepository Staffs { get; private set; }

        public async Task<int> CompleteAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"Lỗi khi lưu dữ liệu vào database: {dbEx.InnerException?.Message}", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi không xác định: {ex.Message}", ex);
            }
        }


        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
