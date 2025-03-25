using QuanLySanBong.Entities.Bill.Dto;

namespace QuanLySanBong.Service.Bill
{
    public interface IBillService
    {
        Task<List<BillDto>> GetAllBillsAsync(int page, int pageSize);
        Task<BillDto> GetBillByIdAsync(int id);
        Task<BillDto> UpdateBillAsync(BillUpdateDto billUpdateDto);
        Task<BillDto> PayBillAsync(int billId, int paidById);

        // 🔹 Xuất hóa đơn giấy (HTML cho Admin & Staff)
        Task<string> GenerateBillHtmlAsync(int billId);

        // 🔹 Xuất hóa đơn PDF (cho Customer)
        Task<byte[]> ExportBillPdfAsync(int billId);
    }
}
