using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuanLySanBong.Entities.PitchType.Dto;
using QuanLySanBong.Entities.PitchType.Model;
using QuanLySanBong.Service.File;
using QuanLySanBong.UnitOfWork;

namespace QuanLySanBong.Service.PitchType
{
    public class PitchTypeService : IPitchTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public PitchTypeService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<List<PitchTypeDto>> GetAllAsync()
        {
            var pitchtypes = await _unitOfWork.PitchTypes.GetAllAsync();
            return _mapper.Map<List<PitchTypeDto>>(pitchtypes);
        }

        public async Task<PitchTypeDto> GetByIdAsync(int id)
        {
            var pitchtype = await _unitOfWork.PitchTypes.GetByIdAsync(id);
            if (pitchtype == null) throw new KeyNotFoundException("PitchType not found.");

            return _mapper.Map<PitchTypeDto>(pitchtype);
        }

        public async Task<PitchTypeDto> AddAsync(PitchTypeCreateDto pitchTypeDto)
        {
            var pitchType = _mapper.Map<PitchTypeModel>(pitchTypeDto);

            await _unitOfWork.PitchTypes.AddAsync(pitchType);
            await _unitOfWork.CompleteAsync(); // Đảm bảo có ID để lưu ảnh

            if (pitchTypeDto.ImageFile != null && pitchTypeDto.ImageFile.Any())
            {
                foreach (var file in pitchTypeDto.ImageFile)
                {
                    var imagePath = await _fileService.SaveFileAsync(file);
                    pitchType.Images.Add(new PitchTypeImageModel
                    {
                        ImagePath = imagePath,
                        PitchTypeId = pitchType.Id
                    });
                }
                await _unitOfWork.CompleteAsync();
            }
            return _mapper.Map<PitchTypeDto>(pitchType);
        }

        public async Task<PitchTypeDto> UpdateAsync(int id, PitchTypeUpdateDto pitchTypeDto)
        {
            var existingPitchType = await _unitOfWork.PitchTypes.GetByIdAsync(id);
            if (existingPitchType == null)
                throw new KeyNotFoundException("PitchType not found.");

            _mapper.Map(pitchTypeDto, existingPitchType);

            // **1️⃣ Xóa ảnh cũ nếu user muốn**
            if (pitchTypeDto.DeleteImageIds != null && pitchTypeDto.DeleteImageIds.Any())
            {
                var imagesToDelete = existingPitchType.Images
                    .Where(img => pitchTypeDto.DeleteImageIds.Contains(img.Id))
                    .ToList();

                foreach (var img in imagesToDelete)
                {
                    await _fileService.DeleteFileAsync(img.ImagePath);
                    existingPitchType.Images.Remove(img);
                }
            }

            // **2️⃣ Thêm ảnh mới nếu có**
            if (pitchTypeDto.ImageFile != null && pitchTypeDto.ImageFile.Any())
            {
                foreach (var file in pitchTypeDto.ImageFile)
                {
                    var imagePath = await _fileService.SaveFileAsync(file);
                    existingPitchType.Images.Add(new PitchTypeImageModel
                    {
                        ImagePath = imagePath,
                        PitchTypeId = existingPitchType.Id
                    });
                }
            }

            await _unitOfWork.PitchTypes.UpdateAsync(existingPitchType);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<PitchTypeDto>(existingPitchType);
        }

        public async Task DeleteAsync(int id)
        {
            // Truy vấn loại sân kèm danh sách ảnh
            var pitchType = await _unitOfWork.PitchTypes
                .GetQueryable()
                .Include(pt => pt.Images) // Load ảnh trước khi xóa
                .FirstOrDefaultAsync(pt => pt.Id == id);

            if (pitchType == null)
                throw new KeyNotFoundException("PitchType not found.");

            _unitOfWork.PitchTypeImages.RemoveRange(pitchType.Images);

            // Xóa loại sân (EF Core sẽ tự động xóa ảnh do `Cascade Delete`)
            await _unitOfWork.PitchTypes.DeleteAsync(pitchType);

            // Lưu thay đổi vào database
            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> DeleteImageAsync(int imageId)
        {
            var img = await _unitOfWork.PitchTypeImages.GetByIdAsync(imageId);
            if(imageId == null)
                throw new KeyNotFoundException("Image not found.");

            await _fileService.DeleteFileAsync(img.ImagePath);

            _unitOfWork.PitchTypeImages.Remove(img);
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
