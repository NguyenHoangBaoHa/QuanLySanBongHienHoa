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

        public PitchController(IPitchService service)
        {
            _service = service;
        }

        // GET: api/Pitch
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var pitches = await _service.GetAllAsync();
            return Ok(pitches);
        }

        // GET: api/Pitch/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var pitch = await _service.GetByIdAsync(id);
            if (pitch == null)
            {
                return NotFound(new { message = $"Không tìm thấy sân có ID: {id}" });
            }
            return Ok(pitch);
        }

        // POST: api/Pitch
        [HttpPost]
        public async Task<IActionResult> Add(PitchCreateDto model)
        {
            try
            {
                var pitch = await _service.AddAsync(model);
                return CreatedAtAction(nameof(GetById), new { id = pitch.Id }, pitch);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Pitch/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PitchUpdateDto model)
        {
            try
            {
                await _service.UpdateAsync(id, model);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Không tìm thấy sân có ID: {id}" });
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Pitch/{id}
        [HttpDelete("{id}")]
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
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
