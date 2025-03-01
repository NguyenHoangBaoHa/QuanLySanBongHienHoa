namespace QuanLySanBong.Entities.PitchType.Dto
{
    public class PitchTypeImageDto
    {
        public int Id { get; set; }  // ID của hình ảnh
        public int PitchTypeId { get; set; }  // ID của loại sân bóng liên kết
        public string ImageUrl { get; set; }  // URL của hình ảnh
    }
}
