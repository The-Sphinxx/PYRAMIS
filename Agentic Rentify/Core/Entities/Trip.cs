using Agentic_Rentify.Core.Common;

namespace Agentic_Rentify.Core.Entities;

public class Trip : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string TripType { get; set; } = string.Empty; // Desert Safari, Nile Cruise, Beach Getaway, etc.
    public DateTime? StartDate { get; set; }
    public decimal Price { get; set; }
    public double Rating { get; set; }
    public int TotalReviews { get; set; }
    public int Reviews { get; set; }
    public string Duration { get; set; } = string.Empty; // "4 Days / 3 Nights"
    public string MainImage { get; set; } = string.Empty;
    public int MaxPeople { get; set; }
    public int AvailableSpots { get; set; }
    public bool Featured { get; set; } = false;
    public bool IsFeatured { get; set; } = false;
    public string Status { get; set; } = "Ongoing"; // Ongoing, Completed, Cancelled

    // سيتخزن كـ JSON
    public List<string> Images { get; set; } = [];
    public List<string> AvailableDates { get; set; } = [];
    public List<string> Highlights { get; set; } = [];
    public List<ItineraryDay> Itinerary { get; set; } = []; // Nested Object

    // Reviews
    public TripReviewSummary ReviewSummary { get; set; } = new();
    public List<UserReview> UserReviews { get; set; } = [];

    // Trip Information
    public TripHotelInfo? HotelInfo { get; set; }
    public TripAmenitiesInfo Amenities { get; set; } = new();
}