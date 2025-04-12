using QuanLySanBong.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace QuanLySanBong.Entities.Bill.Dto
{
    public class BillUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public PaymentStatusEnum PaymentStatus { get; set; }
        public PaymentMethodEnum PaymentMethod { get; set; }
        public DateTime? PaidAt { get; set; } = DateTime.UtcNow;
        public int? PaidById { get; set; }
    }
}
