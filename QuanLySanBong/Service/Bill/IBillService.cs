using Microsoft.AspNetCore.Mvc;
using QuanLySanBong.Entities.Bill.Dto;
using QuanLySanBong.Service.Interface;

namespace QuanLySanBong.Service.Bill
{
    public interface IBillService
    {
        Task<List<BillDto>> GetAllBillsAsync(int page, int pageSize);
        Task<BillDto> GetBillByIdAsync(int id);
        Task<ServiceResponse<BillDto>> CreateBillFromBookingAsyns(BillDto dto);
        Task<BillDto> UpdateBillAsync(BillUpdateDto billUpdateDto);
        Task<BillDto> PayBillAsync(int billId, int paidById);
        Task<List<BillDto>> GetBillsByCustomerIdAsync(int customerId);
        Task<BillDto?> GetBillByBookingIdAsync(int bookingId);
        // 🔹 Xuất hóa đơn PDF (cho Customer)
        Task<FileContentResult> ExportBillPdfAsync(int id);
    }
}
