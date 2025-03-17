using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QuanLySanBong.Entities.Booking.Dto;
using QuanLySanBong.Service.Booking;
using System.Security.Claims;

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

        // 📌 Lấy danh sách Booking cho Admin & Staff
        [HttpGet]
        [Authorize(Roles ="Admin,Staff")]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _service.GetAllBookingsAsync();

            return Ok(bookings);
        }

        // 📌 Lấy Booking theo Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var booking = await _service.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return Ok(booking);
        }

        // 📌 Lấy danh sách Booking của chính Customer
        [HttpGet("customer")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetBookingsByCustomer()
        {
            if (User.Identity?.IsAuthenticated != true)
                return Unauthorized();

            var customerIdClaim = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(customerIdClaim) || !int.TryParse(customerIdClaim, out int customerId))
                return Unauthorized("Không tìm thấy thông tin khách hàng.");

            var bookings = await _service.GetBookingsByCustomerIdAsync(customerId);
            return Ok(bookings);
        }

        // 📌 Lấy lịch đặt sân theo tuần
        [HttpGet("pitch/{pitchId}/week")]
        public async Task<IActionResult> GetPitchScheduleByWeek(int pitchId, [FromQuery] DateTime startDate)
        {
            if (startDate < DateTime.Today)
                return BadRequest("Ngày bắt đầu không thể là ngày trong quá khứ.");

            try
            {
                var bookings = await _service.GetBookingsForPitchByWeekAsync(pitchId, startDate);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("CreateBooking")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Customer")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDto bookingDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Check User ID từ Token
            var idCustomer = User.FindFirstValue("IdCustomer"); // Check IdCustomer từ Token

            Console.WriteLine($"📌 User ID từ Token: {userId}");
            Console.WriteLine($"📌 IdCustomer từ Token: {idCustomer}");

            if (string.IsNullOrEmpty(idCustomer) || idCustomer == "0")
            {
                return Unauthorized("⚠️ Không tìm thấy IdCustomer trong token!");
            }

            var customerId = int.Parse(idCustomer); // Lấy customerId từ token
            var result = await _service.CreateBookingAsync(customerId, bookingDto);
            return Ok(result);
        }


        // 📌 Staff cập nhật trạng thái nhận sân
        [HttpPut("{id}/receive")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> UpdateReceivedStatus(int id, [FromBody] bool isReceived)
        {
            var result = await _service.UpdateReceivedStatusAsync(id, isReceived);
            if(!result)
                return NotFound();
            return NoContent();
        }

        // 📌 Xóa Booking
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var result = await _service.DeleteBookingAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
