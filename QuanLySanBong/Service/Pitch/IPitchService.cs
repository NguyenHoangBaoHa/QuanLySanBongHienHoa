using QuanLySanBong.Entities.Pitch.Dto;
using QuanLySanBong.Entities.Pitch.Model;

namespace QuanLySanBong.Service.Pitch
{
    public interface IPitchService
    {
        Task<List<PitchDto>> GetAllAsync();
        Task<List<PitchDto>> GetAllAsync(int pitchTypeId);
        Task<PitchDto> GetByIdAsync(int id);
        Task<PitchModel> AddAsync(PitchCreateDto pitchDto);
        Task UpdateAsync(int id, PitchUpdateDto pitchDto);
        Task DeleteAsync(int id);
    }
}
