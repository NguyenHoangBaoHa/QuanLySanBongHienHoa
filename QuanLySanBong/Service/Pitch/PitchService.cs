using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuanLySanBong.Entities.Enums;
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

        public async Task<List<PitchDto>> GetAllAsync() // <- Thêm lại
        {
            var pitches = await _unitOfWork.Pitches.GetAllAsync();

            var pitchDtos = _mapper.Map<List<PitchDto>>(pitches);
            foreach (var (dto, pitch) in pitchDtos.Zip(pitches))
            {
                dto.ListImagePath = pitch.PitchType.Images.Select(i => i.ImagePath).ToList();
                dto.ImagePath = dto.ListImagePath.FirstOrDefault();
            }
            return pitchDtos;

        }

        public async Task<List<PitchDto>> GetAllAsync(int pitchTypeId)
        {
            var pitches = await _unitOfWork.Pitches
                .GetQueryable()
                .Where(p => p.IdPitchType == pitchTypeId)
                .ToListAsync();

            return _mapper.Map<List<PitchDto>>(pitches);
        }


        public async Task<PitchDto> GetByIdAsync(int id)
        {
            var pitch = await _unitOfWork.Pitches.GetByIdAsync(id);
            if (pitch == null) throw new KeyNotFoundException("Không tìm thấy sân");

            return _mapper.Map<PitchDto>(pitch);
        }



        public async Task<PitchModel> AddAsync(PitchCreateDto pitchDto)
        {
            if (pitchDto.IdPitchType == null)
                throw new ArgumentNullException(nameof(pitchDto.IdPitchType), "IdPitchType không được để trống.");

            var pitchType = await _unitOfWork.PitchTypes.GetByIdAsync(pitchDto.IdPitchType.Value);
            if (pitchType == null)
                throw new KeyNotFoundException($"Loại sân với Id {pitchDto.IdPitchType.Value} không tồn tại.");

            var pitch = _mapper.Map<PitchModel>(pitchDto);
            pitch.PitchType = pitchType;

            await _unitOfWork.Pitches.AddAsync(pitch);

            return pitch;
        }

        public async Task UpdateAsync(int id, PitchUpdateDto pitchDto)
        {
            var pitch = await _unitOfWork.Pitches.GetByIdAsync(id);
            if (pitch == null) throw new KeyNotFoundException("Không tìm thấy sân.");

            _mapper.Map(pitchDto, pitch);
            await _unitOfWork.Pitches.UpdateAsync(pitch);
        }

        public async Task DeleteAsync(int id)
        {
            var pitch = await _unitOfWork.Pitches.GetByIdAsync(id);
            if (pitch == null) throw new KeyNotFoundException("Không tìm thấy sân");

            await _unitOfWork.Pitches.DeleteAsync(id);
        }
    }
}
