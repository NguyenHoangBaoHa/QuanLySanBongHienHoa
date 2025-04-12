using QuanLySanBong.Entities.Enums;

namespace QuanLySanBong.Entities.Bill.Dto
{
    public class BillDto
    {
        public int Id { get; set; }
        public int IdBooking { get; set; }
        public string DisplayName { get; set; }
        public string PitchName { get; set; }
        public string PitchTypeName { get; set; }
        public DateTime BookingDate { get; set; }
        public int Duration { get; set; } // Thời gian đặt sân (lấy từ Booking)
        public decimal BasePrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice { get; set; }
        public PaymentMethodEnum PaymentMethod { get; set; }
        public PaymentStatusEnum PaymentStatus { get; set; } = PaymentStatusEnum.Unpaid; // Mặc định là chưa thanh toán
        public DateTime? PaidAt { get; set; }
        public string PaidBy { get; set; }
        public DateTime CreatedAt { get; set; }  // ✅ Thêm vào
        public DateTime UpdatedAt { get; set; }  // ✅ Thêm vào

        // Biến tính toán để chuyển đổi sang dạng chuỗi dễ hiển thị
        public string BookingDateFormatted { get; set; }
        public string PaidAtFormatted => PaidAt?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa thanh toán";
        public string TotalPriceFormatted { get; set; } // Hiển thị dưới dạng tiền tệ
    }

}
