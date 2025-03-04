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

            CreateMap<PitchTypeModel, PitchTypeCreateDto>().ReverseMap();
            CreateMap<PitchTypeModel, PitchTypeUpdateDto>().ReverseMap();

            CreateMap<PitchTypeImageModel, PitchTypeImageDto>().ReverseMap();

            // Mapping từ Model sang DTO và ngược lại
            CreateMap<PitchModel, PitchDto>()
                .ForMember(dest => dest.PitchTypeName, opt => opt.MapFrom(src => src.PitchType.Name));

            CreateMap<PitchCreateDto, PitchModel>();
            CreateMap<PitchUpdateDto, PitchModel>();

            CreateMap<BookingModel, BookingDto>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.DisplayName : string.Empty))
                .ForMember(dest => dest.PitchName, opt => opt.MapFrom(src => src.Pitch != null ? src.Pitch.Name : string.Empty))
                .ForMember(dest => dest.PitchTypeName, opt => opt.MapFrom(src => src.Pitch != null && src.Pitch.PitchType != null ? src.Pitch.PitchType.Name : string.Empty))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration)) // Ánh xạ Duration
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus.ToString())); // Chuyển Enum thành chuỗi

            CreateMap<BookingUpdateStatusDto, BookingModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IsReceived, opt => opt.MapFrom(src => src.IsReceived));


            CreateMap<BillModel, BillDto>()
                .ForMember(dest => dest.IdBooking, opt => opt.MapFrom(src => src.IdBooking))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Booking != null && src.Booking.Customer != null ? src.Booking.Customer.DisplayName : string.Empty))
                .ForMember(dest => dest.PitchName, opt => opt.MapFrom(src => src.Booking != null && src.Booking.Pitch != null ? src.Booking.Pitch.Name : string.Empty))
                .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.Booking != null ? src.Booking.BookingDate : default))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Booking != null ? src.Booking.Duration : 0))
                .ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src => src.BasePrice))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus.ToString()))
                .ForMember(dest => dest.PaidAt, opt => opt.MapFrom(src => src.PaidAt))
                .ForMember(dest => dest.PaidBy, opt => opt.MapFrom(src => src.PaidBy != null ? src.PaidBy.Email : string.Empty))
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
                .ForMember(dest => dest.PaidAt, opt => opt.MapFrom(src => src.PaidAt))
                .ForMember(dest => dest.IdPaidBy, opt => opt.MapFrom(src => src.PaidById));

        }
    }
}
