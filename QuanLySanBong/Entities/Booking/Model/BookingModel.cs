using QuanLySanBong.Entities.Customer.Model;
using QuanLySanBong.Entities.Pitch.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySanBong.Entities.Booking.Model
{
    public class BookingModel
    {
        [Key]
        public int Id { get; set; }

        public int IdCustomer {  get; set; }
        [ForeignKey("IdCustomer")]
        public virtual CustomerModel Customer { get; set; }

        public int IdPitch { get; set; }
        [ForeignKey("IdPitch")]
        public virtual PitchModel Pitch { get; set; }

        public DateTime BookingDate { get; set; }

        public string PaymentStatus { get; set; }

        public bool IsReceived { get; set; }

        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
