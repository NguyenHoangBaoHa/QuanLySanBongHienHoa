using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLySanBong.Service.Booking;

namespace QuanLySanBong.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _service;

        public BookingController(IBookingService service)
        {
            _service = service;
        }

        // Admin và Staff: Xem danh sách đặt sân
        [HttpGet]
        [Authorize(Roles ="Admin,Staff")]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _service.GetAllAsync();

            return Ok(bookings);
        }

        // Staff: Cập nhật trạng thái nhận sân
        [HttpPatch("{id}/receive")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> UpdateReceivedStatus(int id, [FromBody] bool isReceived)
        {
            await _service.UpdateReceivedStatusAsync(id, isReceived);
            return NoContent();
        }
    }
}
