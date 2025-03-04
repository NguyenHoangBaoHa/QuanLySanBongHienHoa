using QuanLySanBong.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace QuanLySanBong.Entities.Bill.Dto
{
    public class BillCreateDto
    {
        [Required]
        public int IdBooking { get; set; }

        [Required]
        public decimal BasePrice { get; set; }
        public decimal Discount { get; set; } = 0;

        [Required]
        public PaymentStatusEnum PaymentStatus { get; set; }
    }
}
