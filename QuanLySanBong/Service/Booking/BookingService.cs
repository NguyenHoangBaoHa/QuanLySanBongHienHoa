using AutoMapper;
using QuanLySanBong.Entities.Booking.Dto;
using QuanLySanBong.UnitOfWork;

namespace QuanLySanBong.Service.Booking
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<BookingDto>> GetAllAsync()
        {
            var bookings = await _unitOfWork.Bookings.GetAllAsync();
            return _mapper.Map<List<BookingDto>>(bookings);
        }

        public async Task UpdateReceivedStatusAsync(int id, bool isReceived)
        {
            await _unitOfWork.Bookings.UpdateReceivedStatusAsync(id, isReceived);
        }
    }
}
