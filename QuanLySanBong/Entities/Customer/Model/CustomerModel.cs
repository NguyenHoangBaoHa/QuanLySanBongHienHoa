using QuanLySanBong.Entities.Account.Model;
using QuanLySanBong.Entities.Booking.Model;
using System.ComponentModel.DataAnnotations;

namespace QuanLySanBong.Entities.Customer.Model
{
    public class CustomerModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string DisplayName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(12)]
        public string CCCD { get; set; } // Citizen Identification Number

        [Required]
        public string Gender { get; set; } // "Male" or "Female"

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [MaxLength(256)]
        public string Address { get; set; }

        // Navigation property to Account
        public virtual AccountModel Account { get; set; }

        public virtual ICollection<BookingModel> Bookings { get; set; }
    }
}
