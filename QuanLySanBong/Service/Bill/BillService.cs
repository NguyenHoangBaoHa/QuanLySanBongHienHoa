using AutoMapper;
using QuanLySanBong.Entities.Bill.Dto;
using QuanLySanBong.UnitOfWork;
using System.Reflection.Metadata;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;

namespace QuanLySanBong.Service.Bill
{
    public class BillService : IBillService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BillService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<BillDto>> GetAllBillsAsync(int page, int pageSize)
        {
            var bills = await _unitOfWork.Bills
                .GetQueryable()
                .Include(b => b.Booking)
                    .ThenInclude(bk => bk.Pitch)
                        .ThenInclude(p => p.PitchType)
                .Include(b => b.Booking)
                    .ThenInclude(bk => bk.Customer)
                .Include(b => b.Booking)
                    .ThenInclude(a => a.Staff)
            .ToListAsync();
            //var bills = await _unitOfWork.Bills.GetAllAsync(page, pageSize);
            return _mapper.Map<List<BillDto>>(bills);
        }

        public async Task<BillDto> GetBillByIdAsync(int id)
        {
            var bill = await _unitOfWork.Bills.GetByIdAsync(id);
            return _mapper.Map<BillDto>(bill);
        }

        public async Task<BillDto> UpdateBillAsync(BillUpdateDto billUpdateDto)
        {
            var bill = await _unitOfWork.Bills.GetByIdAsync(billUpdateDto.Id);
            if(bill == null)
            {
                throw new Exception("Bill not found");
            }

            bill.PaymentStatus = billUpdateDto.PaymentStatus;
            
            if(billUpdateDto.PaymentStatus == Entities.Enums.PaymentStatusEnum.DaThanhToan)
            {
                bill.PaidAt = billUpdateDto.PaidAt ?? DateTime.UtcNow;
                bill.IdPaidBy = billUpdateDto.PaidById;
            }
            else
            {
                bill.PaidAt = null;
                bill.IdPaidBy = null;
            }

            await _unitOfWork.Bills.UpdateAsync(bill);

            return _mapper.Map<BillDto>(bill);
        }

        public async Task<BillDto> PayBillAsync(int billId, int paidById)
        {
            var bill = await _unitOfWork.Bills.GetByIdAsync(billId);
            if (bill == null)
            {
                throw new Exception("Bill not found");
            }

            bill.PaymentStatus = Entities.Enums.PaymentStatusEnum.DaThanhToan;
            bill.PaidAt = DateTime.UtcNow;
            bill.IdPaidBy = paidById;

            await _unitOfWork.Bills.UpdateAsync(bill);

            return _mapper.Map<BillDto>(bill);
        }

        public async Task<string> GenerateBillHtmlAsync(int billId)
        {
            var bill = await _unitOfWork.Bills.GetByIdAsync(billId);
            if (bill == null)
            {
                return null;
            }

            string paidByName = "N/A";
            if(bill.PaidBy.Role != null)
            {
                if(bill.PaidBy.Role == "Staff")
                {
                    paidByName = bill.PaidBy.Staff?.DisplayName ?? "Unknown Staff";
                }
                else if (bill.PaidBy.Role == "Admin")
                {
                    paidByName = bill.PaidBy.Email;
                }
            }

            return $@"
            <html>
            <head><title>Hóa Đơn Thanh Toán</title></head>
            <body>
                <h2>HÓA ĐƠN THANH TOÁN</h2>
                <p><strong>Ngày xuất hóa đơn:</strong> {DateTime.UtcNow:dd/MM/yyyy}</p>
                <p><strong>Tên nhân viên xuất hóa đơn:</strong> {paidByName}</p>
                <p><strong>Tên khách hàng:</strong> {bill.Booking.Customer?.DisplayName ?? "N/A"}</p>
                <p><strong>Giá gốc:</strong> {bill.BasePrice:C}</p>
                <p><strong>Tổng tiền:</strong> {bill.TotalPrice:C} {(bill.Booking.BookingDate.Hour >= 18 ? "(+20% phí buổi tối)" : "")}</p>
            </body>
            </html>";
        }

        public async Task<byte[]> ExportBillPdfAsync(int billId)
        {
            var bill = await _unitOfWork.Bills.GetByIdAsync(billId);
            if (bill == null)
            {
                return null;
            }

            string paidByName = "N/A";
            if (bill.PaidBy.Role != null)
            {
                if (bill.PaidBy.Role == "Staff")
                {
                    paidByName = bill.PaidBy.Staff?.DisplayName ?? "Unknown Staff";
                }
                else if (bill.PaidBy.Role == "Admin")
                {
                    paidByName = bill.PaidBy.Email;
                }
            }

            using (MemoryStream ms = new MemoryStream())
            {
                iTextSharp.text.Document doc = new iTextSharp.text.Document();
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();

                doc.Add(new Paragraph("HÓA ĐƠN THANH TOÁN"));
                doc.Add(new Paragraph($"Ngày xuất hóa đơn: {DateTime.UtcNow:dd/MM/yyyy}"));
                doc.Add(new Paragraph($"Tên nhân viên xuất hóa đơn: {paidByName}"));
                doc.Add(new Paragraph($"Tên khách hàng: {bill.Booking.Customer?.DisplayName ?? "N/A"}"));
                doc.Add(new Paragraph($"Giá gốc: {bill.BasePrice:C}"));
                doc.Add(new Paragraph($"Tổng tiền: {bill.TotalPrice:C} {(bill.Booking.BookingDate.Hour >= 18 ? "(+20% phí buổi tối)" : "")}"));

                doc.Close();
                return ms.ToArray();
            }
        }
    }
}
