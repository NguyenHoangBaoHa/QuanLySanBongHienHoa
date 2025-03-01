using AutoMapper;
using QuanLySanBong.Entities.Pitch.Dto;
using QuanLySanBong.Entities.Pitch.Model;
using QuanLySanBong.UnitOfWork;

namespace QuanLySanBong.Service.Pitch
{
    public class PitchService : IPitchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PitchService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<PitchDto>> GetAllAsync()
        
        {
            var pitches = await _unitOfWork.Pitches.GetAllAsync();

            foreach (var pitch in pitches)
            {
                Console.WriteLine($"Pitch ID: {pitch.Id}, Name: {pitch.Name}, PitchType: {(pitch.PitchType?.Name ?? "NULL")}");
            }

            var result =  _mapper.Map<List<PitchDto>>(pitches);
            return result;
        }

        public async Task<PitchDto> GetByIdAsync(int id)
        {
            var pitch = await _unitOfWork.Pitches.GetByIdAsync(id);
            if (pitch == null)
            {
                return null;
            }

            return _mapper.Map<PitchDto>(pitch);
        }

        public async Task<PitchModel> AddAsync(PitchCreateDto pitchDto)
        {
            var pitch = _mapper.Map<PitchModel>(pitchDto);
            await _unitOfWork.Pitches.AddAsync(pitch);
            return pitch;
        }

        public async Task UpdateAsync(int id, PitchUpdateDto pitchDto)
        {
            if(pitchDto.Status != "Trống" && pitchDto.Status != "Đã Đặt")
            {
                throw new ArgumentException("Trạng thái không hợp lệ. Chỉ chấp nhận 'Trống' hoặc 'Đã Đặt'.");
            }

            var pitch = await _unitOfWork.Pitches.GetByIdAsync(id);
            if(pitch == null)
            {
                throw new KeyNotFoundException("Pitch not found");
            }

            // Cập nhật dữ liệu từ DTO sang model
            _mapper.Map(pitchDto, pitch);
            pitch.UpdateAt = DateTime.UtcNow;

            await _unitOfWork.Pitches.UpdateAsync(pitch);
        }

        public async Task DeleteAsync(int id)
        {
            await _unitOfWork.Pitches.DeleteAsync(id);
        }
    }
}
