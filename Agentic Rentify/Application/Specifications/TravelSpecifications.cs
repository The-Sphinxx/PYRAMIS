using System.Linq.Expressions;
using Agentic_Rentify.Core.Entities;

namespace Agentic_Rentify.Application.Specifications;

/// <summary>
/// Hotel Specification for filtering and querying hotels with optimized field selection
/// NOTE: These specifications show the INTENDED usage pattern.
/// Current BaseSpecification implementation uses constructor-based criteria (immutable).
/// For actual usage, use Expression<Func<Hotel, bool>> directly in repository queries.
/// </summary>
public class HotelSpecification : BaseSpecification<Hotel>
{
    public HotelSpecification(string? city = null, decimal? minPrice = null, decimal? maxPrice = null, double? minRating = null, bool? onlyActive = null) 
        : base(BuildCriteria(city, minPrice, maxPrice, minRating, onlyActive))
    {
    }

    private static Expression<Func<Hotel, bool>> BuildCriteria(
        string? city, 
        decimal? minPrice, 
        decimal? maxPrice, 
        double? minRating, 
        bool? onlyActive)
    {
        return h =>
            (string.IsNullOrEmpty(city) || h.City == city) &&
            (!minPrice.HasValue || h.PricePerNight >= minPrice.Value) &&
            (!maxPrice.HasValue || h.PricePerNight <= maxPrice.Value) &&
            (!minRating.HasValue || h.Rating >= minRating.Value) &&
            (!onlyActive.HasValue || !onlyActive.Value || h.Status == "Active") &&
            h.Latitude != 0 && h.Longitude != 0;
    }
}

/// <summary>
/// Car Specification for filtering rental vehicles
/// </summary>
public class CarSpecification : BaseSpecification<Car>
{
    public CarSpecification(
        string? city = null, 
        int? minSeats = null, 
        decimal? minPrice = null, 
        decimal? maxPrice = null, 
        bool? onlyAvailable = null,
        string? type = null)
        : base(BuildCriteria(city, minSeats, minPrice, maxPrice, onlyAvailable, type))
    {
    }

    private static Expression<Func<Car, bool>> BuildCriteria(
        string? city,
        int? minSeats,
        decimal? minPrice,
        decimal? maxPrice,
        bool? onlyAvailable,
        string? type)
    {
        return c =>
            (string.IsNullOrEmpty(city) || c.City == city) &&
            (!minSeats.HasValue || c.Seats >= minSeats.Value) &&
            (!minPrice.HasValue || c.Price >= minPrice.Value) &&
            (!maxPrice.HasValue || c.Price <= maxPrice.Value) &&
            (string.IsNullOrEmpty(type) || c.Type == type) &&
            (!onlyAvailable.HasValue || !onlyAvailable.Value || (c.Status == "Available" && c.AvailableNow > 0));
    }
}

/// <summary>
/// Attraction Specification for filtering tourist attractions
/// </summary>
public class AttractionSpecification : BaseSpecification<Attraction>
{
    public AttractionSpecification(
        string? city = null,
        double? minRating = null,
        bool? onlyActive = null,
        bool? onlyAvailable = null)
        : base(BuildCriteria(city, minRating, onlyActive, onlyAvailable))
    {
    }

    private static Expression<Func<Attraction, bool>> BuildCriteria(
        string? city,
        double? minRating,
        bool? onlyActive,
        bool? onlyAvailable)
    {
        return a =>
            (string.IsNullOrEmpty(city) || a.City == city) &&
            (!minRating.HasValue || a.Rating >= minRating.Value) &&
            (!onlyActive.HasValue || !onlyActive.Value || a.Status == "Active") &&
            (!onlyAvailable.HasValue || !onlyAvailable.Value || a.Availability == "Available") &&
            a.Latitude != 0 && a.Longitude != 0;
    }
}

/// <summary>
/// Trip Specification for filtering trip packages
/// </summary>
public class TripSpecification : BaseSpecification<Trip>
{
    public TripSpecification(
        string? destination = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        double? minRating = null,
        bool? onlyOngoing = null)
        : base(BuildCriteria(destination, minPrice, maxPrice, minRating, onlyOngoing))
    {
    }

    private static Expression<Func<Trip, bool>> BuildCriteria(
        string? destination,
        decimal? minPrice,
        decimal? maxPrice,
        double? minRating,
        bool? onlyOngoing)
    {
        return t =>
            (string.IsNullOrEmpty(destination) || t.Destination == destination || t.City == destination) &&
            (!minPrice.HasValue || t.Price >= minPrice.Value) &&
            (!maxPrice.HasValue || t.Price <= maxPrice.Value) &&
            (!minRating.HasValue || t.Rating >= minRating.Value) &&
            (!onlyOngoing.HasValue || !onlyOngoing.Value || t.Status == "Ongoing");
    }
}

