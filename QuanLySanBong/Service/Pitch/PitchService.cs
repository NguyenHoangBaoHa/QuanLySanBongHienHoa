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
            var pitchDtos = pitches.Select(pitch => new PitchDto
            {
                Id = pitch.Id,
                Name = pitch.Name,
                IdPitchType = pitch.PitchType?.Id ?? 0,
                PitchTypeName = pitch.PitchType?.Name ?? string.Empty,
                Price = pitch.PitchType?.Price ?? 0,
                LimitPerson = pitch.PitchType?.LimitPerson ?? 0,
                ImagePath = pitch.PitchType?.Images.FirstOrDefault()?.ImagePath ?? "default_image.png",
                CreateAt = pitch.CreateAt,
                UpdateAt = pitch.UpdateAt
            }).ToList();

            return pitchDtos;
        }


        public async Task<PitchDto> GetByIdAsync(int id)
        {
            var pitch = await _unitOfWork.Pitches.GetByIdAsync(id);
            if (pitch == null) return null;

            var pitchType = await _unitOfWork.PitchTypes.GetByIdAsync(pitch.IdPitchType ?? 0);
            var images = await _unitOfWork.PitchTypeImages.GetByPitchTypeIdAsync(pitch.IdPitchType ?? 0);

            return new PitchDto
            {
                Id = pitch.Id,
                Name = pitch.Name,
                IdPitchType = pitchType?.Id ?? 0,
                PitchTypeName = pitchType?.Name ?? string.Empty,
                Price = pitchType?.Price ?? 0,
                LimitPerson = pitchType?.LimitPerson ?? 0,
                ListImagePath = images?.Select(img => img.ImagePath).ToList() ?? new List<string>(),
                // ✅ Trả về danh sách ảnh
                CreateAt = pitch.CreateAt,
                UpdateAt = pitch.UpdateAt
            };
        }



        public async Task<PitchModel> AddAsync(PitchCreateDto pitchDto)
        {
            var pitch = _mapper.Map<PitchModel>(pitchDto);
            pitch.CreateAt = DateTime.UtcNow;
            pitch.UpdateAt = DateTime.UtcNow;

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
