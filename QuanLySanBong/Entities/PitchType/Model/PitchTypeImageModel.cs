using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLySanBong.Entities.PitchType.Model
{
    public class PitchTypeImageModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PitchTypeId { get; set; }

        [Required]
        [MaxLength(255)]
        public string ImagePath { get; set; }

        [ForeignKey("PitchTypeId")]
        public PitchTypeModel PitchType { get; set; }
    }
}
