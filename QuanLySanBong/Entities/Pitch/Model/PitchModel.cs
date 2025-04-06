using QuanLySanBong.Entities.PitchType.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using QuanLySanBong.Entities.Booking.Model;
using QuanLySanBong.Entities.Enums;
using System.Text.Json.Serialization;

namespace QuanLySanBong.Entities.Pitch.Model
{
    public class PitchModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public int? IdPitchType { get; set; }

        [ForeignKey("IdPitchType")]
        public virtual PitchTypeModel PitchType { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; } = DateTime.Now;

        [JsonIgnore]
        public virtual ICollection<BookingModel> Bookings { get; set; } = new List<BookingModel>();
    }

}
