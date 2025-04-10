using QuanLySanBong.Entities.Bill.Model;

namespace QuanLySanBong.Repository.Bill
{
    public interface IBillRepository
    {
        Task<IEnumerable<BillModel>> GetAllAsync(int page, int pageSize);
        Task<BillModel> GetByIdAsync(int id);
        Task UpdateAsync(BillModel bill);
        IQueryable<BillModel> GetQueryable();
    }
}
