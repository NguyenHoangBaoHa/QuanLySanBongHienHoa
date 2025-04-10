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
            return Ok(createdPitchType);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromForm] PitchTypeUpdateDto model)
        {
            var updatedPitchType = await _service.UpdateAsync(id, model);
            return Ok(updatedPitchType); // có thể trả NoContent nếu không muốn trả data
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok(new { success = true });
        }
    }

}
