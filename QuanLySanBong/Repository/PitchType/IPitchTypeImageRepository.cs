using Microsoft.EntityFrameworkCore;
using QuanLySanBong.Data;
using QuanLySanBong.Entities.PitchType.Model;
using QuanLySanBong.Repository.Interface;

namespace QuanLySanBong.Repository.PitchType
{
    public interface IPitchTypeImageRepository : IGenericRepository<PitchTypeImageModel>
    {
        Task<IEnumerable<PitchTypeImageModel>> GetByPitchTypeIdAsync(int pitchTypeId);
        void RemoveRange(IEnumerable<PitchTypeImageModel> images);
    }
}
