using AutoMapper;
using QuanLySanBong.Entities.Bill.Dto;
using QuanLySanBong.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using DinkToPdf;
using Microsoft.AspNetCore.Mvc;
using QuanLySanBong.Service.Interface;
using QuanLySanBong.Entities.Bill.Model;

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

        public async Task<ServiceResponse<BillDto>> CreateBillFromBookingAsyns(BillDto dto)
        {
            var billModel = _mapper.Map<BillModel>(dto);

            billModel.PaymentStatus = dto.PaymentStatus;
            billModel.CreatedAt = DateTime.UtcNow;

            var createBill = await _unitOfWork.Bills.CreateAsync(billModel);

            var billDto = _mapper.Map<BillDto>(createBill);

            return new ServiceResponse<BillDto> { Data = billDto};

        }

        public async Task<BillDto> UpdateBillAsync(BillUpdateDto billUpdateDto)
        {
            var bill = await _unitOfWork.Bills.GetByIdAsync(billUpdateDto.Id);
            if (bill == null)
            {
                throw new Exception("Bill not found");
            }

            bill.PaymentStatus = billUpdateDto.PaymentStatus;
            bill.PaymentMethod = billUpdateDto.PaymentMethod;

            if (billUpdateDto.PaymentStatus == Entities.Enums.PaymentStatusEnum.Paid)
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

            bill.PaymentStatus = Entities.Enums.PaymentStatusEnum.Paid;
            bill.PaidAt = DateTime.UtcNow;
            bill.IdPaidBy = paidById;

            await _unitOfWork.Bills.UpdateAsync(bill);

            return _mapper.Map<BillDto>(bill);
        }

        public async Task<List<BillDto>> GetBillsByCustomerIdAsync(int customerId)
        {
            var bills = await _unitOfWork.Bills.GetBillsByCustomerIdAsync(customerId);
            if(bills == null || !bills.Any())
            {
                return new List<BillDto>();
            }

            var billDtos = bills.Select(bill => new BillDto
            {
                Id = bill.Id,
                DisplayName = bill.Booking.Customer.DisplayName,
                PitchName = bill.Booking.Pitch.Name,
                PitchTypeName = bill.Booking.Pitch.PitchType.Name,
                BookingDate = bill.Booking.BookingDate,
                Duration = bill.Booking.Duration,
                BasePrice = bill.BasePrice,
                Discount = bill.Discount,
                TotalPrice = bill.TotalPrice,
                PaymentMethod = bill.PaymentMethod,
                PaymentStatus = bill.PaymentStatus,
                PaidAt = bill.PaidAt,
                PaidBy = bill.PaidBy?.ToString(),
                CreatedAt = bill.CreatedAt,
                UpdatedAt = bill.UpdatedAt,
                BookingDateFormatted = bill.Booking.BookingDate.ToString("dd/MM/yyyy HH:mm"),
                TotalPriceFormatted = bill.TotalPrice.ToString("C")
            }).ToList();

            return billDtos;
        }

        public async Task<BillDto?> GetBillByBookingIdAsync(int bookingId)
        {
            var bill = await _unitOfWork.Bills.GetBillByBookingIdAsync(bookingId);
            if (bill == null) return null;

            return new BillDto
            {
                Id = bill.Id,
                DisplayName = bill.Booking.Customer.DisplayName,
                PitchName = bill.Booking.Pitch.Name,
                PitchTypeName = bill.Booking.Pitch.PitchType.Name,
                BookingDate = bill.Booking.BookingDate,
                Duration = bill.Booking.Duration,
                BasePrice = bill.BasePrice,
                Discount = bill.Discount,
                TotalPrice = bill.TotalPrice,
                PaymentMethod = bill.PaymentMethod,
                PaymentStatus = bill.PaymentStatus,
                PaidAt = bill.PaidAt,
                PaidBy = bill.PaidBy?.ToString(),
                CreatedAt = bill.CreatedAt,
                UpdatedAt = bill.UpdatedAt,
                BookingDateFormatted = bill.Booking.BookingDate.ToString("dd/MM/yyyy HH:mm"),
                TotalPriceFormatted = bill.TotalPrice.ToString("C")
            };
        }

        public async Task<FileContentResult> ExportBillPdfAsync(int id)
        {
            var bill = await _unitOfWork.Bills.GetByIdAsync(id);
            if (bill == null)
            {
                return null;
            }

            string staffname = bill.PaidBy?.Staff?.DisplayName ?? "n/a";
            string customername = bill.Booking.Customer?.DisplayName ?? "n/a";
            string dateexport = DateTime.UtcNow.ToString("dd/mm/yyyy");
            string baseprice = bill.Booking.Pitch.PitchType.Price.ToString("N0") + " vnđ";
            string totalprice = bill.TotalPrice.ToString("N0") + " vnđ";

            string htmlContent = $@"
            <html>
            <head>
                <style>
                    body {{ font-family: arial, sans-serif; line-height: 1.6; }}
                    table {{ width: 100%; border-collapse: collapse; }}
                    td {{ padding: 8px; }}
                    .line {{ border-bottom: 1px dashed #000; margin: 20px 0; }}
                </style>
            </head>
            <body>
                <h1 style='text-align:center;'>sân bóng hiền hòa</h1>
                <h2 style='text-align:center;'>hóa đơn thanh toán</h2>
                <table>
                    <tr>
                        <td><strong>tên nhân viên</strong></td>
                        <td>{staffname}</td>
                        <td><strong>ngày xuất</strong></td>
                        <td>{dateexport}</td>
                    </tr>
                    <tr>
                        <td><strong>tên khách hàng</strong></td>
                        <td colspan='3'>{customername}</td>
                    </tr>
                    <tr>
                        <td><strong>giá sân (giá gốc)</strong></td>
                        <td colspan='3'>{baseprice}</td>
                    </tr>
                    <tr>
                        <td><strong>tổng tiền</strong></td>
                        <td colspan='3'>{totalprice}</td>
                    </tr>
                </table>
                <hr class='line' />
                <table>
                    <tr>
                        <td><strong>thành tiền</strong></td>
                        <td colspan='3'>{totalprice}</td>
                    </tr>
                </table>
            </body>
            </html>";

            var converter = new BasicConverter(new PdfTools());
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                PaperSize = PaperKind.A6, // nhỏ gọn như hóa đơn
                Orientation = Orientation.Portrait,
                Margins = new MarginSettings { Top = 10, Bottom = 10 },
            },
                Objects = {
                    new ObjectSettings {
                        HtmlContent = htmlContent,
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
            };

            byte[] pdf = converter.Convert(doc);
            return new FileContentResult(pdf, "application/pdf")
            {
                FileDownloadName = $"Bill_{bill.Id}.pdf"
            };
        }
    }
}
