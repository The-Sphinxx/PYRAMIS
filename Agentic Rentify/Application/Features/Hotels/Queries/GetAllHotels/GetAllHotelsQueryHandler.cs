using System.Linq.Expressions;
using Agentic_Rentify.Application.Features.Hotels.DTOs;
using Agentic_Rentify.Application.Wrappers;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using AutoMapper;
using MediatR;

namespace Agentic_Rentify.Application.Features.Hotels.Queries.GetAllHotels;

public class GetAllHotelsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) 
    : IRequestHandler<GetAllHotelsQuery, PagedResponse<HotelResponseDTO>>
{
    public async Task<PagedResponse<HotelResponseDTO>> Handle(GetAllHotelsQuery request, CancellationToken cancellationToken)
    {
        // Build filter expression
        var parameter = Expression.Parameter(typeof(Hotel), "h");
        Expression? combinedExpression = null;

        // Search term filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm;
            var nameProperty = Expression.Property(parameter, "Name");
            var descriptionProperty = Expression.Property(parameter, "Description");
            var cityProperty = Expression.Property(parameter, "City");
            
            var nameContains = Expression.Call(nameProperty, "Contains", null, Expression.Constant(searchTerm));
            var descriptionContains = Expression.Call(descriptionProperty, "Contains", null, Expression.Constant(searchTerm));
            var cityContains = Expression.Call(cityProperty, "Contains", null, Expression.Constant(searchTerm));
            
            var searchExpression = Expression.OrElse(Expression.OrElse(nameContains, descriptionContains), cityContains);
            combinedExpression = combinedExpression == null ? searchExpression : Expression.AndAlso(combinedExpression, searchExpression);
        }

        // City filter
        if (!string.IsNullOrWhiteSpace(request.City))
        {
            var city = request.City;
            var cityProperty = Expression.Property(parameter, "City");
            var cityContains = Expression.Call(cityProperty, "Contains", null, Expression.Constant(city));
            combinedExpression = combinedExpression == null ? cityContains : Expression.AndAlso(combinedExpression, cityContains);
        }

        // Status filter
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            var status = request.Status;
            var statusProperty = Expression.Property(parameter, "Status");
            var statusEqual = Expression.Equal(statusProperty, Expression.Constant(status));
            combinedExpression = combinedExpression == null ? statusEqual : Expression.AndAlso(combinedExpression, statusEqual);
        }

        // Featured filter
        if (request.Featured.HasValue)
        {
            var featured = request.Featured.Value;
            var featuredProperty = Expression.Property(parameter, "Featured");
            var isFeaturedProperty = Expression.Property(parameter, "IsFeatured");
            var featuredEqual = Expression.Equal(featuredProperty, Expression.Constant(featured));
            var isFeaturedEqual = Expression.Equal(isFeaturedProperty, Expression.Constant(featured));
            var featuredExpression = Expression.OrElse(featuredEqual, isFeaturedEqual);
            combinedExpression = combinedExpression == null ? featuredExpression : Expression.AndAlso(combinedExpression, featuredExpression);
        }

        // Min rating filter
        if (request.MinRating.HasValue)
        {
            var minRating = request.MinRating.Value;
            var ratingProperty = Expression.Property(parameter, "Rating");
            var ratingGreaterEqual = Expression.GreaterThanOrEqual(ratingProperty, Expression.Constant(minRating));
            combinedExpression = combinedExpression == null ? ratingGreaterEqual : Expression.AndAlso(combinedExpression, ratingGreaterEqual);
        }

        // Max price filter
        if (request.MaxPrice.HasValue)
        {
            var maxPrice = request.MaxPrice.Value;
            var priceProperty = Expression.Property(parameter, "BasePrice");
            var priceLessEqual = Expression.LessThanOrEqual(priceProperty, Expression.Constant(maxPrice));
            combinedExpression = combinedExpression == null ? priceLessEqual : Expression.AndAlso(combinedExpression, priceLessEqual);
        }

        Expression<Func<Hotel, bool>>? filter = null;
        if (combinedExpression != null)
        {
            filter = Expression.Lambda<Func<Hotel, bool>>(combinedExpression, parameter);
        }

        // Use projection to select only what is needed for the list

        // Calculate total count first (without paging)
        var countSpec = new Agentic_Rentify.Application.Specifications.GenericSpecification<Hotel>(filter);
        var totalCount = await unitOfWork.Repository<Hotel>().CountAsync(countSpec);

        // Use projection to select only what is needed for the list
        var dtos = await unitOfWork.Repository<Hotel>().ListAsync(
            new Agentic_Rentify.Application.Specifications.GenericSpecification<Hotel>(
                filter, 
                orderBy: h => h.Id, 
                skip: (request.PageNumber - 1) * request.PageSize, 
                take: request.PageSize, 
                isPagingEnabled: true
            ), 
            h => new HotelResponseDTO
            {
                Id = h.Id,
                Name = h.Name,
                City = h.City,
                Rating = h.Rating,
                ReviewsCount = h.ReviewsCount,
                BasePrice = h.BasePrice + "$", // Assuming currency symbol logic is handled or static
                RawBasePrice = h.BasePrice,
                PricePerNight = h.PricePerNight + "$",
                RawPricePerNight = h.PricePerNight,
                // Do not fetch description for list if not needed, or truncate it
                Description = h.Description.Length > 100 ? h.Description.Substring(0, 100) + "..." : h.Description,
                Status = h.Status,
                Featured = h.Featured,
                IsFeatured = h.IsFeatured,
                // Only take the first image
                Images = h.Images.Take(1).ToList(),
                Amenities = h.Amenities.Take(3).ToList(), // Take top 3 for preview
                ReviewSummary = new HotelReviewSummaryDTO
                {
                    TotalReviews = h.ReviewSummary != null ? h.ReviewSummary.TotalReviews : 0,
                    OverallRating = h.ReviewSummary != null ? h.ReviewSummary.OverallRating : 0
                },
                Latitude = h.Latitude,
                Longitude = h.Longitude
            });

        return new PagedResponse<HotelResponseDTO>(dtos, request.PageNumber, request.PageSize, totalCount);
    }
}
