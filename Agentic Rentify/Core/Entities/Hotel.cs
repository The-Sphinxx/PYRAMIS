using Agentic_Rentify.Core.Common;

namespace Agentic_Rentify.Core.Entities;

public class Hotel : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public double Rating { get; set; }
    public int ReviewsCount { get; set; }
    public decimal BasePrice { get; set; }
    public decimal PricePerNight { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public string Status { get; set; } = "Active"; // Active, Pending, Inactive, Maintenance
    public bool Featured { get; set; } = false;
    public bool IsFeatured { get; set; } = false;

    // Coordinates
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    // Images and Collections - سيتخزن كـ JSON
    public List<string> Images { get; set; } = [];
    public List<string> Amenities { get; set; } = [];
    public List<string> RoomAmenities { get; set; } = [];
    public List<string> Highlights { get; set; } = [];
    public List<string> NearbyLocations { get; set; } = [];
    public List<HotelRoom> Rooms { get; set; } = [];

    // Reviews
    public HotelReviewSummary ReviewSummary { get; set; } = new();
    public List<UserReview> UserReviews { get; set; } = [];

    // Availability
    public HotelAvailability Availability { get; set; } = new();
}