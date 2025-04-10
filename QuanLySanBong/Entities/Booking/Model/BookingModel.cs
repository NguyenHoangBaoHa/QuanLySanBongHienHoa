using QuanLySanBong.Entities.Bill.Model;
using QuanLySanBong.Entities.Customer.Model;
using QuanLySanBong.Entities.Enums;
using QuanLySanBong.Entities.Pitch.Model;
using QuanLySanBong.Entities.Staff.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySanBong.Entities.Booking.Model
{
    public class BookingModel
    {
        [Key]
        public int Id { get; set; }

        public int? IdStaff { get; set; }
        [ForeignKey("IdStaff")]
        public virtual StaffModel Staff { get; set; }

        public int IdCustomer { get; set; }
        [ForeignKey("IdCustomer")]
        public virtual CustomerModel Customer { get; set; }

        public int IdPitch { get; set; }
        [ForeignKey("IdPitch")]
        public virtual PitchModel Pitch { get; set; }

        public DateTime BookingDate { get; set; }
        public int Duration { get; set; } // Thời gian đặt sân (số phút hoặc số giờ)

        public PaymentStatusEnum PaymentStatus { get; set; }

        public bool IsReceived { get; set; }
        public DateTime? ReceivedTime { get; set; }
        public bool IsCanceled { get; set; } // Đảm bảo có logic cập nhật khi hủy

        public virtual BillModel Bill { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

        public TimeslotStatus TimeslotStatus { get; set; }

        public void UpdateTimestamp()
        {
            UpdateAt = DateTime.UtcNow;
        }
    }
}
