using QuanLySanBong.Entities.PitchType.Model;
using QuanLySanBong.Repository.Interface;

namespace QuanLySanBong.Repository.PitchType
{
    public interface IPitchTypeRepository : IGenericRepository<PitchTypeModel>
    {
        Task<IEnumerable<PitchTypeModel>> GetAllAsync();
        Task<PitchTypeModel> GetByIdAsync(int id);
        Task AddAsync(PitchTypeModel pitchType);
        Task UpdateAsync(PitchTypeModel pitchType);
        Task DeleteAsync(PitchTypeModel pitchType);
    }
}
