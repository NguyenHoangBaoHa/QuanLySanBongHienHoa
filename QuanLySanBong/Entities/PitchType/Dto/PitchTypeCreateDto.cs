using System.ComponentModel.DataAnnotations;

namespace QuanLySanBong.Entities.PitchType.Dto
{
    public class PitchTypeCreateDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int LimitPerson { get; set; }
        public List<IFormFile> ImageFile { get; set; } = new(); // Uploaded file
    }
}
