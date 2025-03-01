﻿using System.ComponentModel.DataAnnotations;

namespace QuanLySanBong.Entities.Booking.Dto
{
    public class BookingDto
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string PitchName { get; set; }
        public string PitchTypeName { get; set; }
        public DateTime BookingDate { get; set; }
        public string PaymentStatus { get; set; }
        public bool IsReceived { get; set; }
    }

    public class BookingUpdateStatusDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public bool IsReceived { get; set; }
    }
}
