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
                Console.WriteLine($"Pitch ID: {pitch.Id}, Name: {pitch.Name}, PitchType: {(pitch.PitchType?.Name ?? "NULL")}, ImageCount: {pitch.PitchType?.Images.Count ?? 0}");
            }

            var result = pitches.Select(p => new PitchDto
            {
                Id = p.Id,
                Name = p.Name,
                PitchTypeName = p.PitchType?.Name ?? string.Empty,
                LimitPerson = p.PitchType?.LimitPerson ?? 0,
                Price = p.PitchType?.Price ?? 0,
                ImagePath = (p.PitchType != null && p.PitchType.Images.Any())
                    ? p.PitchType.Images.First().ImagePath
                    : "default_image.png",
                CreateAt = p.CreateAt,
                UpdateAt = p.UpdateAt
            }).ToList();

            return result;
        }

        public async Task<PitchDto> GetByIdAsync(int id)
        {
            var pitch = await _unitOfWork.Pitches.GetByIdAsync(id);
            if (pitch == null)
            {
                return null;
            }

            return new PitchDto
            {
                Id = pitch.Id,
                Name = pitch.Name,
                PitchTypeName = pitch.PitchType?.Name ?? string.Empty,
                LimitPerson = pitch.PitchType?.LimitPerson ?? 0,
                Price = pitch.PitchType?.Price ?? 0,
                ImagePath = pitch.PitchType?.Images?.FirstOrDefault()?.ImagePath,
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
