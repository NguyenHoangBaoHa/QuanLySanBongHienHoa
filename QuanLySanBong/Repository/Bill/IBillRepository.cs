using QuanLySanBong.Entities.Bill.Model;

namespace QuanLySanBong.Repository.Bill
{
    public interface IBillRepository
    {
        Task<IEnumerable<BillModel>> GetAllAsync(int page, int pageSize);
        Task<BillModel> GetByIdAsync(int id);
        Task<BillModel> CreateAsync(BillModel bill);
        Task UpdateAsync(BillModel bill);
        IQueryable<BillModel> GetQueryable();
        Task<List<BillModel>> GetBillsByCustomerIdAsync(int customerId);
        Task<BillModel?> GetBillByBookingIdAsync(int bookingId);
    }
}
