using QuanLySanBong.Entities.Booking.Model;

namespace QuanLySanBong.Repository.Booking
{
    public interface IBookingRepository
    {
        Task<List<BookingModel>> GetAllAsync();
        Task<BookingModel> GetByIdAsync(int id);
        Task UpdateReceivedStatusAsync(int id, bool isReceived);
    }
}
