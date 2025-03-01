using AutoMapper;
using QuanLySanBong.Entities.Booking.Dto;
using QuanLySanBong.Entities.Booking.Model;
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
                .ForMember(dest => dest.PitchTypeName, opt => opt.MapFrom(src => src.Pitch != null && src.Pitch.PitchType != null ? src.Pitch.PitchType.Name : string.Empty));

            CreateMap<BookingUpdateStatusDto, BookingModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IsReceived, opt => opt.MapFrom(src => src.IsReceived));
        }
    }
}
