using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QuanLySanBong.Data;
using QuanLySanBong.Repository.Account;
using QuanLySanBong.Repository.Bill;
using QuanLySanBong.Repository.Booking;
using QuanLySanBong.Repository.Pitch;
using QuanLySanBong.Repository.PitchType;
using QuanLySanBong.Repository.Staff;
using System.Data;

namespace QuanLySanBong.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public UnitOfWork(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection"); // Lấy từ appsettings.json

            Accounts = new AccountRepository(_context);

            PitchTypeImages = new PitchTypeImageRepository(_context);
            
            PitchTypes = new PitchTypeRepository(_context);

            Pitches = new PitchRepository(_context);

            Bookings = new BookingRepository(_context);
            
            Bills = new BillRepository(_context);

            Staffs = new StaffRepository(_context);

        }

        public IAccountRepository Accounts { get; private set; }

        public IPitchTypeImageRepository PitchTypeImages { get; private set; }

        public IPitchTypeRepository PitchTypes { get; private set; }

        public IPitchRepository Pitches { get; private set; }

        public IBookingRepository Bookings { get; private set; }

        public IBillRepository Bills { get; private set; }

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

        // 📌 Thêm phương thức để thực thi Stored Procedure
        public async Task<List<T>> ExecuteStoredProcedureAsync<T>(
            string storedProcedureName,
            object parameters,  // Thêm tham số
            Func<SqlDataReader, T> mapFunction)
        {
            var results = new List<T>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // 🔥 Thêm tham số vào Stored Procedure
                    if (parameters != null)
                    {
                        foreach (var prop in parameters.GetType().GetProperties())
                        {
                            var value = prop.GetValue(parameters);
                            command.Parameters.AddWithValue($"@{prop.Name}", value ?? DBNull.Value);
                        }
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add(mapFunction(reader));
                        }
                    }
                }
            }

            return results;
        }

    }
}
