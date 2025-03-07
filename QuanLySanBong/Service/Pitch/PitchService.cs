using AutoMapper;
using Microsoft.Data.SqlClient;
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
            var result = await _unitOfWork.ExecuteStoredProcedureAsync("GetAllPitches", reader => new PitchDto
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                PitchTypeName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                Price = reader.IsDBNull(3) ? 0 : reader.GetDecimal(3),
                LimitPerson = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                ImagePath = reader.IsDBNull(5) ? "default_image.png" : reader.GetString(5),
                CreateAt = reader.GetDateTime(6),
                UpdateAt = reader.GetDateTime(7)
            });

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
