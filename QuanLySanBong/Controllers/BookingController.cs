using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        [HttpGet("history-bookings")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetBookingsByCustomer()
        {
            var customerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine($"🔍 CustomerId từ token: {customerIdClaim}");

            if (string.IsNullOrEmpty(customerIdClaim) || !int.TryParse(customerIdClaim, out int customerId))
            {
                Console.WriteLine("❌ Không tìm thấy CustomerId trong token.");
                return Unauthorized("Không tìm thấy thông tin khách hàng.");
            }

            var bookings = await _service.GetBookingsByCustomerIdAsync(customerId);
            Console.WriteLine($"✅ Lấy {bookings.Count} bookings cho customer {customerId}");

            return Ok(bookings);
        }

        // 📌 Lấy lịch đặt sân theo tuần
        // 📌 Lấy lịch đặt sân theo tuần kèm trạng thái khung giờ
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
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDto bookingDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Check User ID từ Token
            int idCustomer = bookingDto.IdCustomer; // Check IdCustomer từ Token

            Console.WriteLine($"📌 User ID từ Token: {userId}");
            Console.WriteLine($"📌 IdCustomer từ Token: {idCustomer}");

            if (idCustomer == 0)
            {
                return Unauthorized("⚠️ Không tìm thấy IdCustomer trong token!");
            }

            try
            {
                var result = await _service.CreateBookingAsync(idCustomer, bookingDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // 📌 Staff cập nhật trạng thái nhận sân
        [HttpPatch("{id}/confirm-received")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> UpdateReceivedStatus(int id, [FromBody] BookingUpdateStatusDto model)
        {
            if (model == null || !model.IsReceived == null)
                return BadRequest(new { message = "Thiếu dữ liệu 'isReceived'." });

            var result = await _service.UpdateReceivedStatusAsync(id, model.IsReceived);

            if (!result)
                return BadRequest(new { message = "Cập nhật thất bại." });

            return Ok(new { message = "Cập nhật thành công." });
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

        // 📌 Hủy đặt sân
        [HttpPatch("{id}/cancel")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var userId = User.FindFirstValue("Id");
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int customerId))
                return Unauthorized("Không tìm thấy thông tin khách hàng.");

            try
            {
                bool result = await _service.CancelBookingAsync(customerId, id);
                if (!result)
                {
                    return BadRequest("Không thể hủy đặt sân.");
                }

                return Ok(new { message = "Hủy đặt sân thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
