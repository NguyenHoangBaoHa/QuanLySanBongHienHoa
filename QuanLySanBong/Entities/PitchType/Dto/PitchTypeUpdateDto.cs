namespace QuanLySanBong.Entities.PitchType.Dto
{
    public class PitchTypeUpdateDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int LimitPerson {  get; set; }
        public List<IFormFile>? ImageFile { get; set; }
        public List<int>? DeleteImageIds { get; set; }
    }
}
