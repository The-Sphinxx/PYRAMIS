using System;
using Agentic_Rentify.Application.Features.Attractions.DTOs;
using Agentic_Rentify.Application.Wrappers;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Application.Features.Attractions.Specifications;
using AutoMapper;
using MediatR;

namespace Agentic_Rentify.Application.Features.Attractions.Queries.GetAllAttractions;

public class GetAllAttractionsQueryHandler : IRequestHandler<GetAllAttractionsQuery, PagedResponse<AttractionResponseDTO>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllAttractionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResponse<AttractionResponseDTO>> Handle(GetAllAttractionsQuery request, CancellationToken cancellationToken)
    {
        var spec = new AttractionWithFiltersSpecification(request, applyPaging: true);
        
        // Use projection to select only what is needed for the list
        var dtos = await _unitOfWork.Repository<Attraction>().ListAsync(spec, a => new AttractionResponseDTO
        {
            Id = a.Id,
            Name = a.Name,
            City = a.City,
            Rating = a.Rating,
            Price = a.Price + " " + a.Currency,
            RawPrice = a.Price,
            // Only take the first image to save bandwidth/db load
            Images = a.Images.Select(i => i.Url).Take(1).ToList(),
            Categories = a.Categories,
            Reviews = new ReviewSummaryDTO 
            { 
               TotalReviews = a.ReviewSummary != null ? a.ReviewSummary.TotalReviews : 0,
               OverallRating = a.ReviewSummary != null ? a.ReviewSummary.OverallRating : 0
            },
            // Do not fetch heavy UserReviews or Description for the list
            Description = a.Description.Length > 100 ? a.Description.Substring(0, 100) + "..." : a.Description,
            Availability = a.Availability,
            IsFeatured = a.IsFeatured
        });

        var countSpec = new AttractionWithFiltersSpecification(request, applyPaging: false);
        var totalCount = await _unitOfWork.Repository<Attraction>().CountAsync(countSpec);

        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Max(1, request.PageSize);

        return new PagedResponse<AttractionResponseDTO>(dtos, pageNumber, pageSize, totalCount);
    }
}
