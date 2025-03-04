using QuanLySanBong.Entities.Account.Model;
using QuanLySanBong.Entities.Booking.Model;
using QuanLySanBong.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySanBong.Entities.Bill.Model
{
    public class BillModel
    {
        [Key]
        public int Id { get; set; }

        public int IdBooking { get; set; }
        [ForeignKey("IdBooking")]
        public virtual BookingModel Booking { get; set; }

        public decimal BasePrice { get; set; }
        public decimal Discount { get; set; } = 0;
        public decimal TotalPrice { get; set; }

        [Required]
        public PaymentMethodEnum PaymentMethod { get; set; }

        [Required]
        public PaymentStatusEnum PaymentStatus { get; set; }

        public DateTime? PaidAt { get; set; }

        public int? IdPaidBy { get; set; }
        [ForeignKey("IdPaidBy")]
        public virtual AccountModel PaidBy {  get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        //Cập nhật TotalPrice
        public void CalculateTotalPrice()
        {
            TotalPrice = BasePrice - Discount;
        }

        //Cập nhật tự động khi có thay đổi
        public void UpdateTimestamp()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
