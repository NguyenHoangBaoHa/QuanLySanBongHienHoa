namespace QuanLySanBong.Entities.Bill.Dto
{
    public class BillExportDto
    {
        public string StaffName { get; set; }               // ✅ Account.PaidBy.Staff.DisplayName
        public string CustomerName { get; set; }            // ✅ Booking.Account.Customer.DisplayName
        public string CustomerPhoneNumber { get; set; }

        public string PitchName { get; set; }
        public string PitchTypeName { get; set; }
        public DateTime BookingDate { get; set; }
        public int Duration { get; set; }

        public decimal BasePrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice { get; set; }

        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime? PaidAt { get; set; }

        public string PaidAtFormatted => PaidAt?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa thanh toán";
        public string TotalPriceFormatted => TotalPrice.ToString("N0") + " VNĐ";

        // Thêm chuỗi định dạng ngày đặt
        public string BookingDateFormatted => BookingDate.ToString("dd/MM/yyyy HH:mm");
    }
}
