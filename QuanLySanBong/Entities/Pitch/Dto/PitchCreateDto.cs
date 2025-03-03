using System.ComponentModel.DataAnnotations;

namespace QuanLySanBong.Entities.Pitch.Dto
{
    public class PitchCreateDto
    {
        [Required]
        public string Name { get; set; }
        public int? IdPitchType { get; set; }
    }

    public class PitchUpdateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; }

        public int? IdPitchType { get; set; }
    }
}
