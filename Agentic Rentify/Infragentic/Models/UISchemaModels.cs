namespace Agentic_Rentify.Infragentic.Models;

/// <summary>
/// UI Schema Models - These DTOs are PRECISELY mapped to Vue.js component props
/// to ensure 100% synchronization between backend and frontend.
/// </summary>

// ============================================================
// HOTEL CARD UI SCHEMA (HotelCard.vue)
// ============================================================
public class HotelCardDTO
{
    /// <summary>
    /// Hotel name (displayed as title)
    /// Vue: {{ hotel.name }}
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Main image URL (single image, NOT array)
    /// Vue: :src="hotel.image"
    /// </summary>
    public string Image { get; set; } = string.Empty;

    /// <summary>
    /// Price per night (numeric for calculations)
    /// Vue: ${{ hotel.price }}
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Location/City name
    /// Vue: {{ hotel.location }}
    /// </summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Rating (numeric, e.g., 4.8)
    /// Vue: {{ hotel.rating }}
    /// </summary>
    public double Rating { get; set; }

    /// <summary>
    /// Reviews count (formatted string, e.g., "5,205")
    /// Vue: {{ hotel.reviews }} reviews
    /// </summary>
    public string Reviews { get; set; } = string.Empty;

    /// <summary>
    /// List of amenities (e.g., ["Wifi", "Pool", "Gym"])
    /// Vue: v-for="amenity in hotel.amenities"
    /// </summary>
    public List<string> Amenities { get; set; } = new();
}

// ============================================================
// CAR CARD UI SCHEMA (CarCard.vue)
// ============================================================
public class CarCardDTO
{
    /// <summary>
    /// Car name/model
    /// Vue: {{ car.name }}
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Overview/Description
    /// Vue: {{ car.overview }}
    /// </summary>
    public string Overview { get; set; } = string.Empty;

    /// <summary>
    /// Array of image URLs (NOT single image)
    /// Vue: :src="getImage(car.images)"
    /// </summary>
    public List<string> Images { get; set; } = new();

    /// <summary>
    /// Price per day (numeric)
    /// Vue: ${{ car.price }}
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Reviews object (NOT flat)
    /// Vue: {{ car.reviews.overallRating }} ({{ car.reviews.totalReviews }} reviews)
    /// </summary>
    public CarReviewsDTO Reviews { get; set; } = new();
}

public class CarReviewsDTO
{
    public double OverallRating { get; set; }
    public int TotalReviews { get; set; }
}

// ============================================================
// ATTRACTION CARD UI SCHEMA (AttractionCard.vue)
// ============================================================
public class AttractionCardDTO
{
    /// <summary>
    /// Attraction ID
    /// Vue: :id="id"
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Main image URL (single image)
    /// Vue: :src="image"
    /// </summary>
    public string Image { get; set; } = string.Empty;

    /// <summary>
    /// Attraction title/name
    /// Vue: {{ title }}
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Price (can be string or number)
    /// Vue: {{ price }}
    /// </summary>
    public string Price { get; set; } = string.Empty;

    /// <summary>
    /// Location/City
    /// Vue: {{ location }}
    /// </summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Rating (numeric)
    /// Vue: {{ rating }}
    /// </summary>
    public double Rating { get; set; }

    /// <summary>
    /// Reviews count (numeric, NOT string)
    /// Vue: {{ reviews.toLocaleString() }}
    /// </summary>
    public int Reviews { get; set; }
}

// ============================================================
// TRIP CARD UI SCHEMA (TripCard.vue)
// ============================================================
public class TripCardDTO
{
    /// <summary>
    /// Trip ID
    /// Vue: trip.id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Trip title
    /// Vue: {{ trip.title }}
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Trip description
    /// Vue: {{ trip.description }}
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Main image URL (single image)
    /// Vue: :src="trip.image"
    /// </summary>
    public string Image { get; set; } = string.Empty;

    /// <summary>
    /// Price (numeric for calculations)
    /// Vue: {{ trip.price.toLocaleString() }} $
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Rating (numeric)
    /// Vue: {{ trip.rating }}
    /// </summary>
    public double Rating { get; set; }

    /// <summary>
    /// Duration string (e.g., "4 Days / 3 Nights")
    /// Vue: {{ trip.duration }}
    /// </summary>
    public string Duration { get; set; } = string.Empty;

    /// <summary>
    /// City/Location
    /// Vue: {{ trip.city }}
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Amenities object (NOT array)
    /// Vue: trip.amenities.transport, trip.amenities.accommodation, trip.amenities.meals
    /// </summary>
    public TripAmenitiesDTO Amenities { get; set; } = new();
}

public class TripAmenitiesDTO
{
    public bool Transport { get; set; }
    public bool Accommodation { get; set; }
    public int Meals { get; set; }
}

// ============================================================
// TRIP PLANNER RESPONSE SCHEMA (PlannerResult.vue)
// ============================================================
public class TripPlannerResponseDTO
{
    public TripOverviewDTO TripOverview { get; set; } = new();
    public EstimatedCostsDTO EstimatedCosts { get; set; } = new();
    public List<LodgingRecommendationDTO> LodgingRecommendations { get; set; } = new();
    public VehicleRecommendationDTO? VehicleRecommendation { get; set; }
    public List<ItineraryDayDTO> Itinerary { get; set; } = new();
    public List<string> TravelTips { get; set; } = new();
}

public class TripOverviewDTO
{
    public string Destination { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty; // yyyy-MM-dd
    public string EndDate { get; set; } = string.Empty; // yyyy-MM-dd
    public int DurationDays { get; set; }
    public int Travelers { get; set; }
    public string BudgetLevel { get; set; } = string.Empty;
}

public class EstimatedCostsDTO
{
    public decimal AccommodationTotal { get; set; }
    public decimal ActivitiesTotal { get; set; }
    public decimal TransportationTotal { get; set; }
    public decimal GrandTotal { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal PerPerson { get; set; }
}

public class LodgingRecommendationDTO
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

public class VehicleRecommendationDTO
{
    public int VehicleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Seats { get; set; }
    public decimal PricePerDay { get; set; }
    public int TotalDays { get; set; }
    public decimal TotalCost { get; set; }
    public List<string> Images { get; set; } = new();
}

public class ItineraryDayDTO
{
    public int Day { get; set; }
    public string Date { get; set; } = string.Empty; // yyyy-MM-dd
    public string Title { get; set; } = string.Empty;
    public List<ActivityDTO> Activities { get; set; } = new();
    public MealsDTO? Meals { get; set; }
}

public class ActivityDTO
{
    public string Time { get; set; } = string.Empty; // e.g., "09:00"
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationHours { get; set; }
    public decimal Price { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string ActivityType { get; set; } = "attraction"; // "attraction" or "activity"
}

public class MealsDTO
{
    public string Breakfast { get; set; } = string.Empty;
    public string Lunch { get; set; } = string.Empty;
    public string Dinner { get; set; } = string.Empty;
}
