using QuanLySanBong.Entities.PitchType.Dto;
using QuanLySanBong.Entities.PitchType.Model;

namespace QuanLySanBong.Service.PitchType
{
    public interface IPitchTypeService
    {
        Task<List<PitchTypeDto>> GetAllAsync();
        Task<PitchTypeDto> GetByIdAsync(int id);
        Task<PitchTypeDto> AddAsync(PitchTypeCreateDto pitchType);
        Task<PitchTypeDto> UpdateAsync(int id, PitchTypeUpdateDto pitchTypeDto);
        Task DeleteAsync(int id);
        Task<bool> DeleteImageAsync(int imageId);
    }
}
