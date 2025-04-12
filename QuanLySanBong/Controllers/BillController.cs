using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLySanBong.Entities.Bill.Dto;
using QuanLySanBong.Service.Bill;

namespace QuanLySanBong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly IBillService _service;
        private readonly ILogger<BillController> _logger;

        public BillController(IBillService service, ILogger<BillController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // 🔹 Lấy danh sách hóa đơn (Admin & Staff có quyền)
        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAllBills([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var bills = await _service.GetAllBillsAsync(page, pageSize);

                if (bills == null || !bills.Any())
                {
                    return NotFound("Không có hóa đơn nào.");
                }

                var billDtos = bills.Select(bill => new BillDto
                {
                    Id = bill.Id,
                    IdBooking = bill.IdBooking,
                    DisplayName = bill.DisplayName,
                    PitchName = bill.PitchName,
                    PitchTypeName = bill.PitchTypeName,
                    BookingDate = bill.BookingDate,
                    Duration = bill.Duration,
                    BasePrice = bill.BasePrice,
                    Discount = bill.Discount,
                    TotalPrice = bill.TotalPrice,
                    PaymentMethod = bill.PaymentMethod,
                    PaymentStatus = bill.PaymentStatus,
                    PaidAt = bill.PaidAt,
                    PaidBy = bill.PaidBy?.ToString(),
                    CreatedAt = bill.CreatedAt,
                    UpdatedAt = bill.UpdatedAt,
                    // Sửa lại cách trả về định dạng cho BookingDateFormatted
                    BookingDateFormatted = bill.BookingDate.ToString("dd/MM/yyyy HH:mm"),
                    TotalPriceFormatted = bill.TotalPrice.ToString("C")
                }).ToList();

                return Ok(billDtos);
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết để debug
                _logger.LogError($"Lỗi khi lấy danh sách hóa đơn: {ex.Message}", ex);
                return StatusCode(500, "Có lỗi xảy ra trong quá trình xử lý yêu cầu.");
            }
        }

        // 🔹 Lấy chi tiết hóa đơn theo ID
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetBillById(int id)
        {
            var bill = await _service.GetBillByIdAsync(id);
            if(bill == null)
            {
                return NotFound(new { Message = "Bill not found" });
            }
            return Ok(bill);
        }

        [HttpPost("create")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateBill([FromBody] BillDto dto)
        {
            try
            {
                if (dto == null || dto.IdBooking <= 0)
                {
                    return BadRequest(new { Message = "Dữ liệu không hợp lệ." });
                }

                // Gọi service để tạo hóa đơn
                var response = await _service.CreateBillFromBookingAsyns(dto);

                if (response.Data == null)
                {
                    return BadRequest(new { Message = "Tạo hóa đơn thất bại." });
                }

                return Ok(response.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi tạo hóa đơn: {ex.Message}", ex);
                return StatusCode(500, "Đã xảy ra lỗi khi tạo hóa đơn.");
            }
        }

        // 🔹 Cập nhật trạng thái thanh toán (Admin & Staff)
        [HttpPut]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> UpdateBill([FromBody] BillUpdateDto billUpdateDto)
        {
            try
            {
                var updateBill = await _service.UpdateBillAsync(billUpdateDto);
                return Ok(updateBill);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // 🔹 Thanh toán hóa đơn (Admin hoặc Staff thanh toán giúp Customer)
        [HttpPost("pay/{billId}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> PayBill(int billId, [FromBody] int paidById)
        {
            try
            {
                var paidBill = await _service.PayBillAsync(billId, paidById);

                return Ok(paidBill);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("customer/transactions")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetTransactionHistory()
        {
            try
            {
                var customerIdClaim = User.Claims.FirstOrDefault(c => c.Type == "IdCusstomer");
                if(customerIdClaim == null)
                {
                    return Unauthorized(new { Message = "Không tìm thấy thông tin khách hàng." });
                }

                var customerId = int.Parse(customerIdClaim.Value);

                var bills = await _service.GetBillsByCustomerIdAsync(customerId);

                return Ok(bills);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi lấy lịch sử giao dịch: {ex.Message}", ex);
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý yêu cầu.");
            }
        }

        [HttpGet("booking/{bookingId}")]
        [Authorize(Roles = "Customer,Staff,Admin")]
        public async Task<IActionResult> GetBillByBookingId(int bookingId)
        {
            var bill = await _service.GetBillByBookingIdAsync(bookingId);
            if (bill == null)
            {
                return NotFound(new { Message = "Không tìm thấy hóa đơn cho booking này." });
            }
            return Ok(bill);
        }

        [HttpGet("{id}/pdf")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> GetbillPdf(int id)
        {
            var billPdf = await _service.ExportBillPdfAsync(id);
            if (billPdf == null)
            {
                return NotFound(new { Message = "Bill not found" });
            }

            return billPdf;
        }
    }
}
