using QuanLySanBong.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace QuanLySanBong.Entities.Booking.Dto
{
    public class BookingCreateDto
    {
        [Required]
        public int IdCustomer { get; set; }

        [Required]
        public int IdPitch { get; set; } // Sân mà khách đặt

        [Required]
        public DateTime BookingDate { get; set; } // Ngày giờ đặt sân

        [Required]
        [Range(30, 180, ErrorMessage = "Thời gian đặt sân phải từ 30 đến 180 phút.")]
        public int Duration { get; set; } // Thời gian đặt sân (tính theo phút)

        public PaymentStatusEnum PaymentStatus { get; set; } = PaymentStatusEnum.ChuaThanhToan; // Mặc định là chưa thanh toán
    }
}
