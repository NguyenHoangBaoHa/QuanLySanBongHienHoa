using AutoMapper;
using QuanLySanBong.Entities.Bill.Dto;
using QuanLySanBong.Entities.Bill.Model;
using QuanLySanBong.Entities.Booking.Dto;
using QuanLySanBong.Entities.Booking.Model;
using QuanLySanBong.Entities.Enums;
using QuanLySanBong.Entities.Pitch.Dto;
using QuanLySanBong.Entities.Pitch.Model;
using QuanLySanBong.Entities.PitchType.Dto;
using QuanLySanBong.Entities.PitchType.Model;

namespace QuanLySanBong.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PitchTypeModel, PitchTypeDto>()
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.Images != null ? src.Images.Select(img => img.ImagePath).ToList() : new List<string>()))
                .ReverseMap();

            CreateMap<PitchTypeCreateDto, PitchTypeModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.LimitPerson, opt => opt.MapFrom(src => src.LimitPerson))
                .ForMember(dest => dest.Images, opt => opt.Ignore()); // không ánh xạ ImageFile

            CreateMap<PitchTypeUpdateDto, PitchTypeModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.LimitPerson, opt => opt.MapFrom(src => src.LimitPerson))
                .ForMember(dest => dest.Images, opt => opt.Ignore()); // không ánh xạ ImageFile


            CreateMap<PitchTypeImageModel, PitchTypeImageDto>().ReverseMap();

            // Mapping từ Model sang DTO
            CreateMap<PitchModel, PitchDto>()
                .ForMember(dest => dest.PitchTypeName, opt => opt.MapFrom(src => src.PitchType.Name))
                .ForMember(dest => dest.IdPitchType, opt => opt.MapFrom(src => src.PitchType.Id))
                .ForMember(dest => dest.LimitPerson, opt => opt.MapFrom(src => src.PitchType.LimitPerson)) // Thêm dòng này
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.PitchType.Price)) // Và dòng này
                .ForMember(dest => dest.ListImagePath, opt => opt.MapFrom(src =>
                    src.PitchType.Images != null
                    ? src.PitchType.Images.Select(i => i.ImagePath).ToList()
                    : new List<string>()));

            // Mapping từ CreateDto sang Model (mặc định Status là Available)
            CreateMap<PitchCreateDto, PitchModel>()
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // Mapping từ UpdateDto sang Model (có thể cập nhật trạng thái)
            CreateMap<PitchUpdateDto, PitchModel>()
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // Ánh xạ từ BookingCreateDto sang BookingModel
            CreateMap<BookingCreateDto, BookingModel>()
                .ForMember(dest => dest.IdCustomer, opt => opt.MapFrom(src => src.IdCustomer)) // CustomerId -> IdCustomer
                .ForMember(dest => dest.IdPitch, opt => opt.MapFrom(src => src.IdPitch)) // IdPitch
                .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.BookingDate)) // BookingDate
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration)) // Duration
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus)) // PaymentStatus (đã có enum)
                .ForMember(dest => dest.IsReceived, opt => opt.MapFrom(src => false)) // Đặt mặc định IsReceived là false khi tạo mới
                .ForMember(dest => dest.IsCanceled, opt => opt.MapFrom(src => false)) // Thêm IsCanceled với giá trị mặc định là false
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => DateTime.UtcNow)) // Tạo thời gian tạo
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => DateTime.UtcNow)); // Tạo thời gian cập nhật

            // Ánh xạ từ BookingModel sang BookingDto (để trả về cho frontend)
            CreateMap<BookingModel, BookingDto>()
                .ForMember(dest => dest.IdPitch, opt => opt.MapFrom(src => src.IdPitch))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.DisplayName : string.Empty))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.PhoneNumber : string.Empty))
                .ForMember(dest => dest.PitchName, opt => opt.MapFrom(src => src.Pitch != null ? src.Pitch.Name : string.Empty))
                .ForMember(dest => dest.PitchTypeName, opt => opt.MapFrom(src => src.Pitch != null && src.Pitch.PitchType != null ? src.Pitch.PitchType.Name : string.Empty))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus.ToString())) // Chuyển Enum thành chuỗi
                .ForMember(dest => dest.IsCanceled, opt => opt.MapFrom(src => src.IsCanceled)) // Thêm ánh xạ IsCanceled
                .ForMember(dest => dest.TimeslotStatus, opt => opt.MapFrom(src => src.TimeslotStatus)) // Thêm ánh xạ TimeslotStatus
                .ForMember(dest => dest.ReceivedTime, opt => opt.MapFrom(src => src.ReceivedTime));

            // Ánh xạ từ BookingUpdateStatusDto sang BookingModel khi cập nhật trạng thái
            CreateMap<BookingUpdateStatusDto, BookingModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IsReceived, opt => opt.MapFrom(src => src.IsReceived))
                .ForMember(dest => dest.IsCanceled, opt => opt.MapFrom(src => src.IsCanceled)); // Thêm ánh xạ IsCanceled



            CreateMap<BillModel, BillDto>()
                .ForMember(dest => dest.IdBooking, opt => opt.MapFrom(src => src.IdBooking))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src =>
                    src.Booking != null && src.Booking.Customer != null && src.Booking.Customer.Account != null
                        ? src.Booking.Customer.DisplayName
                        : string.Empty))
                .ForMember(dest => dest.PitchName, opt => opt.MapFrom(src =>
                    src.Booking != null && src.Booking.Pitch != null
                        ? src.Booking.Pitch.Name
                        : string.Empty))
                .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src =>
                    src.Booking != null ? src.Booking.BookingDate : default))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src =>
                    src.Booking != null ? src.Booking.Duration : 0))
                .ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src => src.BasePrice))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus.ToString()))
                .ForMember(dest => dest.PaidAt, opt => opt.MapFrom(src => src.PaidAt))
                .ForMember(dest => dest.PaidBy, opt => opt.MapFrom(src =>
                    src.PaidBy != null && src.PaidBy.Staff != null
                        ? src.PaidBy.Staff.DisplayName
                        : (src.PaidBy != null ? src.PaidBy.Email : string.Empty)))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));


            CreateMap<BillCreateDto, BillModel>()
                .ForMember(dest => dest.IdBooking, opt => opt.MapFrom(src => src.IdBooking))
                .ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src => src.BasePrice))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));


            CreateMap<BillUpdateDto, BillModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.PaidAt, opt => opt.MapFrom(src => src.PaidAt))
                .ForMember(dest => dest.IdPaidBy, opt => opt.MapFrom(src => src.PaidById));

            CreateMap<BillModel, BillExportDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src =>
                    src.Booking != null && src.Booking.Customer != null && src.Booking.Customer.Account != null
                        ? src.Booking.Customer.DisplayName
                        : string.Empty))
                .ForMember(dest => dest.CustomerPhoneNumber, opt => opt.MapFrom(src =>
                    src.Booking != null && src.Booking.Customer != null
                        ? src.Booking.Customer.PhoneNumber
                        : string.Empty))
                .ForMember(dest => dest.PitchName, opt => opt.MapFrom(src =>
                    src.Booking != null && src.Booking.Pitch != null
                        ? src.Booking.Pitch.Name
                        : string.Empty))
                .ForMember(dest => dest.PitchTypeName, opt => opt.MapFrom(src =>
                    src.Booking != null && src.Booking.Pitch != null && src.Booking.Pitch.PitchType != null
                        ? src.Booking.Pitch.PitchType.Name
                        : string.Empty))
                .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src =>
                    src.Booking != null
                        ? src.Booking.BookingDate.ToString("dd/MM/yyyy HH:mm")
                        : string.Empty))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus.ToString()))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.ToString()));

        }
    }
}
