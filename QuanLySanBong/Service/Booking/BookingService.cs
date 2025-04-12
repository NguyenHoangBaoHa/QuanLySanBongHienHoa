using AutoMapper;
using QuanLySanBong.Entities.Booking.Dto;
using QuanLySanBong.Entities.Booking.Model;
using QuanLySanBong.Entities.Enums;
using QuanLySanBong.Entities.Pitch.Dto;
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
            var bookings = await _unitOfWork.Bookings.GetAllBookingsAsync();
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }

        // 📌 Lấy Booking theo Id
        public async Task<BookingDto> GetBookingByIdAsync(int id)
        {
            var booking = await _unitOfWork.Bookings.GetBookingByIdAsync(id);
            return _mapper.Map<BookingDto>(booking);
        }

        // 📌 Lấy danh sách Booking của chính Customer
        public async Task<List<BookingDto>> GetBookingsByCustomerIdAsync(int customerId)
        {
            return await _unitOfWork.Bookings.GetBookingsByCustomerIdAsync(customerId);
        }

        // 📌 Lấy danh sách Booking của một sân theo tuần
        public async Task<object> GetBookingsForPitchByWeekAsync(int pitchId, DateTime startDate)
        {
            DateTime endDate = startDate.AddDays(6);

            var bookings = await _unitOfWork.Bookings.GetBookingsByPitchAndDateRangeAsync(pitchId, startDate, endDate);

            var pitch = await _unitOfWork.Pitches.GetByIdAsync(pitchId);
            if (pitch == null)
                throw new Exception("Không tìm thấy sân.");

            return new
            {
                Pitch = _mapper.Map<PitchDto>(pitch),
                Booking = _mapper.Map<IEnumerable<BookingDto>>(bookings)
            };
        }

        // 📌 Thêm Booking mới
        public async Task<BookingDto> CreateBookingAsync(int customerId, BookingCreateDto bookingDto)
        {
            var customerExists = await _unitOfWork.Accounts.GetById(customerId);
            if (customerExists == null)
            {
                throw new Exception($"Khách hàng với Id {customerId} không tồn tại.");
            }

            // Kiểm tra sân có tồn tại không
            var pitch = await _unitOfWork.Pitches.GetByIdAsync(bookingDto.IdPitch);
            if (pitch == null)
                throw new KeyNotFoundException("Không tìm thấy sân.");

            // Kiểm tra ngày đặt sân không được ở quá khứ
            if (bookingDto.BookingDate < DateTime.UtcNow)
                throw new Exception("Ngày đặt sân không hợp lệ.");

            // Kiểm tra trùng khung giờ đặt sân
            bool isAvailable = await _unitOfWork.Bookings.IsTimeSlotAvailable(bookingDto.IdPitch, bookingDto.BookingDate, bookingDto.Duration);
            if (!isAvailable)
                throw new Exception("Khung giờ này không trống hoặc đang bảo trì. Vui lòng chọn khung giờ khác.");

            // Tạo Booking mới
            var booking = new BookingModel
            {
                IdCustomer = customerId,
                IdPitch = bookingDto.IdPitch,
                BookingDate = bookingDto.BookingDate,
                Duration = bookingDto.Duration,
                PaymentStatus = PaymentStatusEnum.Unpaid, // Mặc định chưa thanh toán
                TimeslotStatus = TimeslotStatus.Booked,
                IsReceived = false,
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            };

            await _unitOfWork.Bookings.AddBookingAsync(booking);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<BookingDto>(booking); // Trả về BookingDto sau khi tạo
        }

        // 📌 Cập nhật trạng thái nhận sân của Staff
        public async Task<bool> UpdateReceivedStatusAsync(int bookingId, bool isReceived)
        {
            Console.WriteLine($"📌 Đang cập nhật Booking ID: {bookingId}, IsReceived: {isReceived}");

            var booking = await _unitOfWork.Bookings.GetBookingByIdAsync(bookingId);
            if (booking == null)
            {
                Console.WriteLine("⚠️ Không tìm thấy Booking!");
                return false;
            }

            booking.IsReceived = isReceived;
            booking.ReceivedTime = isReceived ? DateTime.UtcNow.AddHours(7) : null;
            booking.UpdateTimestamp();

            _unitOfWork.Bookings.UpdateBooking(booking);
            await _unitOfWork.CompleteAsync();

            Console.WriteLine("✅ Cập nhật thành công!");
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

        // 📌 Hủy đặt sân
        public async Task<bool> CancelBookingAsync(int customerId, int bookingId)
        {
            var booking = await _unitOfWork.Bookings.GetBookingByIdAsync(bookingId);

            if (booking == null || booking.IdCustomer != customerId)
            {
                return false; // Không tìm thấy hoặc không phải của khách hàng này
            }

            DateTime now = DateTime.UtcNow;
            if (booking.BookingDate <= now.AddHours(1)) // Kiểm tra thời gian hủy
            {
                throw new Exception("Bạn chỉ có thể hủy đặt sân trước ít nhất 1 giờ.");
            }

            if (booking.TimeslotStatus == TimeslotStatus.Booked)
            {
                throw new Exception("Khung giờ này đã bị đặt và không thể hủy.");
            }

            bool isCanceled = await _unitOfWork.Bookings.CancelBookingAsync(bookingId);
            if (!isCanceled)
            {
                throw new Exception("Không thể hủy đặt sân.");
            }

            // Cập nhật trạng thái TimeslotStatus về Available
            booking.TimeslotStatus = TimeslotStatus.Available;
            _unitOfWork.Bookings.UpdateBooking(booking);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public Task<IEnumerable<BookingDto>> GetReceivedBookingsAsync()
        {
            var bookings = _unitOfWork.Bookings.GetReceivedBookingsAsync();
            return bookings;
        }
    }
}
