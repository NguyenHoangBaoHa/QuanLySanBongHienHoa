using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLySanBong.Entities.Pitch.Dto;
using QuanLySanBong.Service.Pitch;

namespace QuanLySanBong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Staff")]
    public class PitchController : ControllerBase
    {
        private readonly IPitchService _service;
        private readonly ILogger<PitchController> _logger;

        public PitchController(IPitchService service, ILogger<PitchController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // ✅ [MỞ QUYỀN] Cho phép tất cả người dùng xem danh sách sân
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var pitches = await _service.GetAllAsync();
            return Ok(pitches);
        }

        // ✅ [MỞ QUYỀN] Cho phép tất cả người dùng xem chi tiết sân
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var pitch = await _service.GetByIdAsync(id);
            if (pitch == null)
            {
                return NotFound(new { message = $"Không tìm thấy sân có ID: {id}" });
            }
            return Ok(pitch);
        }

        // ✅ [GIỮ QUYỀN] Chỉ Admin & Staff mới có thể tạo sân
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Add([FromBody] PitchCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu đầu vào không hợp lệ", errors = ModelState });
            }

            try
            {
                var pitch = await _service.AddAsync(model);
                return CreatedAtAction(nameof(GetById), new { id = pitch.Id }, pitch);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi thêm sân: {ex.Message}");
                return BadRequest(new { message = "Lỗi khi thêm sân", error = ex.Message });
            }
        }

        // ✅ [GIỮ QUYỀN] Chỉ Admin & Staff mới có thể cập nhật sân
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Update(int id, [FromBody] PitchUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu đầu vào không hợp lệ", errors = ModelState });
            }

            try
            {
                await _service.UpdateAsync(id, model);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Không tìm thấy sân có ID: {id}" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi cập nhật sân {id}: {ex.Message}");
                return BadRequest(new { message = "Lỗi khi cập nhật sân", error = ex.Message });
            }
        }

        // ✅ [GIỮ QUYỀN] Chỉ Admin & Staff mới có thể xóa sân
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Không tìm thấy sân có ID: {id}" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi xóa sân {id}: {ex.Message}");
                return BadRequest(new { message = "Lỗi khi xóa sân", error = ex.Message });
            }
        }
    }
}
