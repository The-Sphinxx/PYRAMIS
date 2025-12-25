using System;
using Agentic_Rentify.Application.Features.Trips.DTOs;
using Agentic_Rentify.Application.Wrappers;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Application.Features.Trips.Specifications;
using AutoMapper;
using MediatR;

namespace Agentic_Rentify.Application.Features.Trips.Queries.GetAllTrips;

public class GetAllTripsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) 
    : IRequestHandler<GetAllTripsQuery, PagedResponse<TripResponseDTO>>
{
    public async Task<PagedResponse<TripResponseDTO>> Handle(GetAllTripsQuery request, CancellationToken cancellationToken)
    {
        // Create specification with filters and paging
        var spec = new TripWithFiltersSpecification(request, applyPaging: true);
        
        // Use projection to select only what is needed for the list
        var dtos = await unitOfWork.Repository<Trip>().ListAsync(spec, t => new TripResponseDTO 
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description.Length > 100 ? t.Description.Substring(0, 100) + "..." : t.Description,
            City = t.City,
            Destination = t.Destination,
            TripType = t.TripType,
            StartDate = t.StartDate,
            Price = t.Price + "$", 
            RawPrice = t.Price,
            Rating = t.Rating,
            TotalReviews = t.TotalReviews,
            Reviews = t.TotalReviews,
            Duration = t.Duration,
            MainImage = t.MainImage,
            Images = t.Images.Take(1).ToList(),
            MaxPeople = t.MaxPeople,
            AvailableSpots = t.AvailableSpots,
            Status = t.Status,
            Featured = t.Featured,
            IsFeatured = t.IsFeatured,
            AvailableDates = t.AvailableDates.Take(3).ToList(),
            ReviewSummary = new TripReviewSummaryDTO 
            {
                 TotalReviews = t.ReviewSummary != null ? t.ReviewSummary.TotalReviews : 0,
                 OverallRating = t.ReviewSummary != null ? t.ReviewSummary.OverallRating : 0
            }
        });

        var countSpec = new TripWithFiltersSpecification(request, applyPaging: false);
        var totalCount = await unitOfWork.Repository<Trip>().CountAsync(countSpec);

        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Max(1, request.PageSize);

        return new PagedResponse<TripResponseDTO>(dtos, pageNumber, pageSize, totalCount);
    }
}
