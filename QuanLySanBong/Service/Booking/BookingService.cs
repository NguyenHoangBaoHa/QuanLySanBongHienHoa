using AutoMapper;
using QuanLySanBong.Entities.Booking.Dto;
using QuanLySanBong.Entities.Booking.Model;
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

        // 📌 Lấy danh sách Booking cho Admin & Staff
        public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync()
        {
            var bookings = await _unitOfWork.Bookings.GetAllBookingAsync();
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }

        // 📌 Lấy Booking theo Id
        public async Task<BookingDto> GetBookingByIdAsync(int id)
        {
            var booking = await _unitOfWork.Bookings.GetBookingByIdAsync(id);
            return _mapper.Map<BookingDto>(booking);
        }

        // 📌 Lấy danh sách Booking của chính Customer
        public async Task<IEnumerable<BookingDto>> GetBookingsByCustomerIdAsync(int customerId)
        {
            var bookings = await _unitOfWork.Bookings.GetBookingsByCustomerIdAsync(customerId);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }

        // 📌 Lấy danh sách Booking của một sân theo tuần
        public async Task<IEnumerable<BookingDto>> GetBookingsForPitchByWeekAsync(int pitchId, DateTime startDate)
        {
            if(startDate < DateTime.Today)
            {
                throw new ArgumentException("Ngày bắt đầu không thể là ngày trong quá khứ.");
            }

            DateTime endDate = startDate.AddDays(6); // Lấy từ ngày bắt đầu đến hết tuần (7 ngày)
            var bookings = await _unitOfWork.Bookings.GetBookingsByPitchAndDateRangeAsync(pitchId, startDate, endDate);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }

        // 📌 Thêm Booking mới
        public async Task<BookingDto> CreateBookingAsync(BookingCreateDto bookingDto)
        {
            var booking = _mapper.Map<BookingModel>(bookingDto);
            await _unitOfWork.Bookings.AddBookingAsync(booking);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<BookingDto>(booking);
        }

        // 📌 Cập nhật trạng thái nhận sân của Staff
        public async Task<bool> UpdateReceivedStatusAsync(int bookingId, bool isReceived)
        {
            var booking = await _unitOfWork.Bookings.GetBookingByIdAsync(bookingId);
            if (booking == null) return false;

            booking.IsReceived = isReceived;
            booking.UpdateTimestamp();

            _unitOfWork.Bookings.UpdateBooking(booking);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        // 📌 Xóa Booking
        public async Task<bool> DeleteBookingAsync(int id)
        {
            var booking = await _unitOfWork.Bookings.GetBookingByIdAsync(id);
            if (booking == null) return false;

            _unitOfWork.Bookings.DeleteBooking(booking);
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
