using Agentic_Rentify.Application.Features.Hotels.DTOs;
using Agentic_Rentify.Core.Entities;
using AutoMapper;

namespace Agentic_Rentify.Application.Mappings;

public class HotelProfile : Profile
{
    public HotelProfile()
    {
        CreateMap<Hotel, HotelResponseDTO>()
            .ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src => $"{src.BasePrice} $"))
            .ForMember(dest => dest.RawBasePrice, opt => opt.MapFrom(src => src.BasePrice))
            .ForMember(dest => dest.PricePerNight, opt => opt.MapFrom(src => $"{src.PricePerNight} $"))
            .ForMember(dest => dest.RawPricePerNight, opt => opt.MapFrom(src => src.PricePerNight))
            .ForMember(dest => dest.ReviewsCount, opt => opt.MapFrom(src => src.ReviewSummary.TotalReviews));

        CreateMap<HotelRoom, HotelRoomDTO>().ReverseMap();
        CreateMap<HotelReviewSummary, HotelReviewSummaryDTO>();
        CreateMap<RatingCriteria, RatingCriteriaDTO>();
        CreateMap<UserReview, UserReviewDTO>();
        CreateMap<HotelAvailability, HotelAvailabilityDTO>();

        CreateMap<CreateHotelDTO, Hotel>();
        CreateMap<UpdateHotelDTO, Hotel>();
    }
}
