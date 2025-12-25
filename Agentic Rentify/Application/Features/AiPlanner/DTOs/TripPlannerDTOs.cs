namespace Agentic_Rentify.Application.Features.AiPlanner.DTOs;

public class TripSearchCriteria
{
    public string Destination { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string BudgetLevel { get; set; } = "mid-range"; // budget, mid-range, luxury
    public int Travelers { get; set; } = 1;
    public List<string> Interests { get; set; } = new();
}

public class TripPlanResponse
{
    public TripOverview TripOverview { get; set; } = new();
    public EstimatedCosts EstimatedCosts { get; set; } = new();
    public List<LodgingRecommendation> LodgingRecommendations { get; set; } = new();
    public VehicleRecommendation? VehicleRecommendation { get; set; }
    public List<DayItinerary> Itinerary { get; set; } = new();
    public List<string> TravelTips { get; set; } = new();
    public BookingSummary BookingSummary { get; set; } = new();
    public bool Success { get; set; } = true;
    public string? ErrorMessage { get; set; }
}

public class TripOverview
{
    public string Destination { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public int Travelers { get; set; }
    public string BudgetLevel { get; set; } = string.Empty;
}

public class EstimatedCosts
{
    public decimal AccommodationTotal { get; set; }
    public decimal ActivitiesTotal { get; set; }
    public decimal TransportationTotal { get; set; }
    public decimal GrandTotal { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal PerPerson { get; set; }
}

public class LodgingRecommendation
{
    public int HotelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int TotalNights { get; set; }
    public decimal TotalCost { get; set; }
    public double Rating { get; set; }
    public List<string> Amenities { get; set; } = new();
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public List<string> Images { get; set; } = new();
    public string Reason { get; set; } = string.Empty;
}

public class VehicleRecommendation
{
    public int VehicleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Seats { get; set; }
    public decimal PricePerDay { get; set; }
    public int TotalDays { get; set; }
    public decimal TotalCost { get; set; }
    public List<string> Features { get; set; } = new();
    public List<string> Images { get; set; } = new();
}

public class DayItinerary
{
    public int Day { get; set; }
    public string Date { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<Activity> Activities { get; set; } = new();
    public DayMeals Meals { get; set; } = new();
    public AccommodationInfo Accommodation { get; set; } = new();
}

public class Activity
{
    public string Time { get; set; } = string.Empty;
    public string ActivityType { get; set; } = string.Empty; // attraction, meal, transport, free_time
    public int? AttractionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal DurationHours { get; set; }
    public decimal Price { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public List<string> Images { get; set; } = new();
}

public class DayMeals
{
    public string Breakfast { get; set; } = string.Empty;
    public string Lunch { get; set; } = string.Empty;
    public string Dinner { get; set; } = string.Empty;
}

public class AccommodationInfo
{
    public int HotelId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class BookingSummary
{
    public List<int> HotelsToBook { get; set; } = new();
    public List<int> AttractionsToBook { get; set; } = new();
    public int? VehicleToBook { get; set; }
}
