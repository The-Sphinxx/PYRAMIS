using Agentic_Rentify.Core.Common;

namespace Agentic_Rentify.Core.Entities;

public class Car : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; } = DateTime.Now.Year;
    public string City { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // sedan, SUV, luxury, MPV, Van
    public decimal Price { get; set; }
    public decimal BasePrice { get; set; }
    
    // Car Specifications
    public int Seats { get; set; }
    public string Transmission { get; set; } = "automatic";
    public string FuelType { get; set; } = "Petrol"; // Petrol, Diesel, Hybrid, Electric
    
    // Inventory
    public int TotalFleet { get; set; } = 20;
    public int AvailableNow { get; set; }
    public string NextAvailability { get; set; } = string.Empty;
    
    // Status
    public string Status { get; set; } = "Available"; // Available, Rented, Maintenance, Pending
    public bool Featured { get; set; } = false;
    public bool IsFeatured { get; set; } = false;

    // سيتخزن كـ JSON
    public List<string> Features { get; set; } = [];
    public List<string> Images { get; set; } = [];
    public List<string> Amenities { get; set; } = [];

    // Reviews
    public CarReviewSummary ReviewSummary { get; set; } = new();
    public List<UserReview> UserReviews { get; set; } = [];
}