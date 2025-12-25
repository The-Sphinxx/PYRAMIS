using System.Linq.Expressions;
using Agentic_Rentify.Application.Features.Cars.DTOs;
using Agentic_Rentify.Application.Wrappers;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using AutoMapper;
using MediatR;

namespace Agentic_Rentify.Application.Features.Cars.Queries.GetAllCars;

public class GetAllCarsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) 
    : IRequestHandler<GetAllCarsQuery, PagedResponse<CarResponseDTO>>
{
    public async Task<PagedResponse<CarResponseDTO>> Handle(GetAllCarsQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Car, bool>>? filter = null;
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim();
            filter = c =>
                (c.Name != null && c.Name.Contains(term)) ||
                (c.City != null && c.City.Contains(term)) ||
                (c.Brand != null && c.Brand.Contains(term)) ||
                (c.Model != null && c.Model.Contains(term));
        }

        // Calculate total count first (without paging)
        var countSpec = new Agentic_Rentify.Application.Specifications.GenericSpecification<Car>(filter);
        var totalCount = await unitOfWork.Repository<Car>().CountAsync(countSpec);

        // Use projection to select only what is needed for the list
        var dtos = await unitOfWork.Repository<Car>().ListAsync(
            new Agentic_Rentify.Application.Specifications.GenericSpecification<Car>(
                filter, 
                orderBy: c => c.Id, 
                skip: (request.PageNumber - 1) * request.PageSize, 
                take: request.PageSize, 
                isPagingEnabled: true
            ), 
            c => new CarResponseDTO
            {
                Id = c.Id,
                Name = c.Name,
                Brand = c.Brand,
                Model = c.Model,
                Year = c.Year,
                City = c.City,
                Description = c.Description.Length > 100 ? c.Description.Substring(0, 100) + "..." : c.Description,
                Type = c.Type,
                Seats = c.Seats,
                Transmission = c.Transmission,
                FuelType = c.FuelType,
                Price = c.Price + "$",
                RawPrice = c.Price,
                TotalFleet = c.TotalFleet,
                AvailableNow = c.AvailableNow,
                Status = c.Status,
                Featured = c.Featured,
                IsFeatured = c.IsFeatured,
                Images = c.Images.Take(1).ToList(),
                ReviewSummary = new CarReviewSummaryDTO
                {
                    TotalReviews = c.ReviewSummary != null ? c.ReviewSummary.TotalReviews : 0,
                    OverallRating = c.ReviewSummary != null ? c.ReviewSummary.OverallRating : 0
                }
            });

        return new PagedResponse<CarResponseDTO>(dtos, request.PageNumber, request.PageSize, totalCount);
    }
}
