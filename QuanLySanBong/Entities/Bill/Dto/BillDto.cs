namespace QuanLySanBong.Entities.Bill.Dto
{
    public class BillDto
    {
        public int Id { get; set; }
        public int IdBooking { get; set; }
        public string CustomerName { get; set; }
        public string PitchName { get; set; }
        public DateTime BookingDate { get; set; }
        public int Duration { get; set; } // Thời gian đặt sân (lấy từ Booking)
        public decimal BasePrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? PaidAt { get; set; }
        public string PaidBy { get; set; }
        public DateTime CreatedAt { get; set; }  // ✅ Thêm vào
        public DateTime UpdatedAt { get; set; }  // ✅ Thêm vào
    }

}
