using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using QuanLySanBong.Entities.Customer.Model;
using QuanLySanBong.Entities.Staff.Model;

namespace QuanLySanBong.Entities.Account.Model
{
    public class AccountModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required]
        [MaxLength(256)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // Admin, Customer, or Staff

        // Foreign key references
        public int? IdCustomer { get; set; }
        public int? IdStaff { get; set; }

        // Navigation properties
        [ForeignKey("IdCustomer")]
        public virtual CustomerModel Customer { get; set; }

        [ForeignKey("IdStaff")]
        public virtual StaffModel Staff { get; set; }
    }
}
