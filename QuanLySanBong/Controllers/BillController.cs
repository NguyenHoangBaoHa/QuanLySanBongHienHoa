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

                if(bills == null || !bills.Any())
                {
                    return NotFound("Không có hóa đơn nào.");
                }

                var billDtos = bills.Select(bill => new BillDto
                {
                    Id = bill.Id,
                    DisplayName = bill.DisplayName,
                    PitchName = bill.PitchName,
                    PitchTypeName = bill.PitchTypeName,
                    BookingDate = bill.BookingDate,
                    Duration = bill.Duration,
                    BasePrice = bill.BasePrice,
                    Discount = bill.Discount,
                    TotalPrice = bill.TotalPrice,
                    PaymentMethod = bill.PaymentMethod.ToString(),
                    PaymentStatus = bill.PaymentStatus.ToString(),
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

        // 🔹 Cập nhật trạng thái thanh toán (Admin & Staff)
        [HttpPut]
        [Authorize(Roles = "Admin,Staff")]
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

        [HttpGet("{billId}/html")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetbillHtml(int billId)
        {
            var billHtml = await _service.GenerateBillHtmlAsync(billId);
            if (string.IsNullOrEmpty(billHtml))
            {
                return NotFound(new { Message = "Bill not found" });
            }

            return Content(billHtml, "text/html");
        }

        [HttpGet("{billId}/pdf")]
        [Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetbillPdf(int billId)
        {
            var billPdf = await _service.ExportBillPdfAsync(billId);
            if (billPdf == null)
            {
                return NotFound(new { Message = "Bill not found" });
            }

            return File(billPdf, "application/pdf", "Bill.pdf");
        }
    }
}
