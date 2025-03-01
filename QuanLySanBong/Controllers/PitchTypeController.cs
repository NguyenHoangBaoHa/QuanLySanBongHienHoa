using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLySanBong.Entities.PitchType.Dto;
using QuanLySanBong.Entities.PitchType.Model;
using QuanLySanBong.Service.File;
using QuanLySanBong.Service.PitchType;
using QuanLySanBong.UnitOfWork;

namespace QuanLySanBong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PitchTypeController : ControllerBase
    {
        private readonly IPitchTypeService _service;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public PitchTypeController(IPitchTypeService service, IUnitOfWork unitOfWork)
        {
            _service = service;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetById(int id)
        {
            var pitchType = await _service.GetByIdAsync(id);
            return Ok(pitchType);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromForm] PitchTypeCreateDto model)
        {
            var createdPitchType = await _service.AddAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = createdPitchType.Id }, createdPitchType);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromForm] PitchTypeUpdateDto model)
        {
            var pitchType = await _unitOfWork.PitchTypes.GetByIdAsync(id);
            if (pitchType == null) throw new Exception("Loại sân không tồn tại!");

            // Cập nhật thông tin loại sân
            pitchType.Name = model.Name;
            pitchType.Price = model.Price;
            pitchType.LimitPerson = model.LimitPerson;
            _unitOfWork.PitchTypes.UpdateAsync(pitchType);

            if(model.ImageFile != null)
            {
                foreach(var image in model.ImageFile)
                {
                    string imageUrl = await _fileService.SaveFileAsync(image);
                    var pitchTypeImage = new PitchTypeImageModel
                    {
                        PitchTypeId = id,
                        ImagePath = imageUrl,
                    };
                    await _unitOfWork.PitchTypeImages.AddAsync(pitchTypeImage);
                }
            }

            if(model.DeleteImageIds != null)
            {
                foreach(var imageId in model.DeleteImageIds)
                {
                    var image = await _unitOfWork.PitchTypeImages.GetByIdAsync(imageId);
                    if (image != null)
                    {
                        _unitOfWork.PitchTypeImages.Remove(image);
                    }
                }
            }

            await _unitOfWork.CompleteAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var pitchType = await _unitOfWork.PitchTypes.GetByIdAsync(id);
            if (pitchType == null) return NotFound();

            var images = await _unitOfWork.PitchTypeImages.GetByPitchTypeIdAsync(id);
            foreach (var img in images)
            {
                _unitOfWork.PitchTypeImages.Remove(img);
            }
            _unitOfWork.PitchTypes.DeleteAsync(pitchType);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
    }

}
