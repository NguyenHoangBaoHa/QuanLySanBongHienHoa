using QuanLySanBong.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace QuanLySanBong.Entities.Pitch.Dto
{
    public class PitchCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "IdPitchType không được để trống.")]
        public int? IdPitchType { get; set; }
    }

    public class PitchUpdateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "IdPitchType không được để trống.")]
        public int? IdPitchType { get; set; }
    }
}
