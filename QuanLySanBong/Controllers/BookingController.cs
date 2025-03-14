﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLySanBong.Entities.Booking.Dto;
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

            var bookings = await _service.GetBookingByIdAsync(customerId);
            return Ok(bookings);
        }

        // 📌 Lấy lịch đặt sân theo tuần
        [HttpGet("pitch/{pitchId}/week")]
        public async Task<IActionResult> GetPitchScheduleByWeek(int pitchId, [FromQuery] DateTime startDate)
        {
            if (startDate < DateTime.Today)
                return BadRequest("Ngày bắt đầu không thể là ngày trong quá khứ.");

            var bookings = await _service.GetBookingsForPitchByWeekAsync(pitchId, startDate);
            return Ok(bookings);
        }

        // 📌 Tạo Booking mới
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDto bookingDto)
        {
            if (bookingDto == null || bookingDto.IdPitch <= 0 || bookingDto.BookingDate < DateTime.Now)
                return BadRequest("Dữ liệu đặt sân không hợp lệ.");

            var booking = await _service.CreateBookingAsync(bookingDto);
            return CreatedAtAction(nameof(GetBookingById), new {id = booking.Id}, booking);
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
