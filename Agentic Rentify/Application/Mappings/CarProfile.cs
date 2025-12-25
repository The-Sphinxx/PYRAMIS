using Agentic_Rentify.Application.Features.Cars.DTOs;
using Agentic_Rentify.Core.Entities;
using AutoMapper;

namespace Agentic_Rentify.Application.Mappings;

public class CarProfile : Profile
{
    public CarProfile()
    {
        CreateMap<Car, CarResponseDTO>()
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => $"{src.Price} $"))
            .ForMember(dest => dest.RawPrice, opt => opt.MapFrom(src => src.Price));

        CreateMap<CarReviewSummary, CarReviewSummaryDTO>();
        CreateMap<CarRatingCriteria, CarRatingCriteriaDTO>();
        CreateMap<UserReview, UserReviewDTO>();

        CreateMap<CreateCarDTO, Car>();
        CreateMap<UpdateCarDTO, Car>();
    }
}
