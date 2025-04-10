using QuanLySanBong.Entities.Pitch.Model;

namespace QuanLySanBong.Repository.Pitch
{
    public interface IPitchRepository
    {
        Task<List<PitchModel>> GetAllAsync();
        Task<PitchModel> GetByIdAsync(int id);
        Task AddAsync(PitchModel pitch);
        Task UpdateAsync(PitchModel pitch);
        Task DeleteAsync(int id);

        IQueryable<PitchModel> GetQueryable();
    }
}
