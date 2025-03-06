﻿namespace QuanLySanBong.Entities.Pitch.Dto
{
    public class PitchDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PitchTypeName { get; set; }
        public int LimitPerson {  get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }  // Lấy ảnh đầu tiên
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
