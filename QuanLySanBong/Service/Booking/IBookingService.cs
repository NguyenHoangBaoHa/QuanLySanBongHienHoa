using QuanLySanBong.Entities.Booking.Dto;

namespace QuanLySanBong.Service.Booking
{
    public interface IBookingService
    {
        Task<List<BookingDto>> GetAllAsync();
        Task UpdateReceivedStatusAsync(int id, bool isReceived);
    }
}
