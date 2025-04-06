using QuanLySanBong.Entities.Pitch.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace QuanLySanBong.Entities.PitchType.Model
{
    public class PitchTypeModel
    {
        [Key] // Có thể bỏ đi nếu EF Core tự động nhận diện
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0.")]
        [Column(TypeName = "decimal(18,2)")] // Định nghĩa kiểu dữ liệu rõ ràng trong DB
        public decimal Price { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "LimitPerson must be between 1 and 100.")]
        public int LimitPerson { get; set; }

        [InverseProperty("PitchType")]
        [JsonIgnore]
        public virtual ICollection<PitchModel> Pitches { get; set; } = new List<PitchModel>();

        public virtual ICollection<PitchTypeImageModel> Images { get; set; } = new List<PitchTypeImageModel>();
    }
}
