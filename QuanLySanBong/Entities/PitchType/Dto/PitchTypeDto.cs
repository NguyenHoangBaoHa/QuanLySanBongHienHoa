namespace QuanLySanBong.Entities.PitchType.Dto
{
    public class PitchTypeDto
    {
        public int Id { get; set; } // ID dùng cho Update
        public string Name { get; set; } // Loại sân
        public decimal Price { get; set; } // Giá thuê sân
        public int LimitPerson { get; set; } // Số lượng người tối đa
        public List<string> ImageUrls { get; set; }
    }
}
