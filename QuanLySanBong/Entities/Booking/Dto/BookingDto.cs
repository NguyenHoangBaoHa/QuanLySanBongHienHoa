using QuanLySanBong.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace QuanLySanBong.Entities.Booking.Dto
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int IdCustomer { get; set; }
        public string DisplayName { get; set; }
        public string PhoneNumber { get; set; }
        public string PitchName { get; set; }
        public string PitchTypeName { get; set; }
        public DateTime BookingDate { get; set; }
        public int Duration { get; set; } // Thời gian đặt sân
        public PaymentStatusEnum PaymentStatus { get; set; }
        public bool IsReceived { get; set; }
        public bool IsCanceled { get; set; } // Thêm thuộc tính IsCanceled

        // Biến tính toán để chuyển đổi sang dạng chuỗi dễ hiển thị
        public string BookingDateFormatted => BookingDate.ToString("dd/MM/yyyy HH:mm");
    }

    public class BookingUpdateStatusDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public bool IsReceived { get; set; }
        [Required]
        public bool IsCanceled { get; set; }
    }
}
