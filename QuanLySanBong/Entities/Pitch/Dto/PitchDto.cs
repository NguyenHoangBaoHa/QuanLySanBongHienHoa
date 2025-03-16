namespace QuanLySanBong.Entities.Pitch.Dto
{
    public class PitchDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int IdPitchType { get; set; }
        public string PitchTypeName { get; set; }
        public int LimitPerson {  get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
        public List<string> ListImagePath { get; set; } = new List<string>();
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
