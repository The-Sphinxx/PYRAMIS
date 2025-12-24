using Agentic_Rentify.Application.Features.Attractions.DTOs;
using Agentic_Rentify.Application.Features.Cars.DTOs;
using Agentic_Rentify.Application.Features.Trips.DTOs;
using Agentic_Rentify.Application.Features.Hotels.DTOs;

namespace Agentic_Rentify.Application.Features.AiPlanner.DTOs;

/// <summary>
/// معايير البحث التي يرسلها المستخدم من الواجهة الأمامية
/// </summary>
public class TripSearchCriteria
{
    public string HeadingTo { get; set; } = string.Empty; 
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Travelers { get; set; }
    public string WithWhom { get; set; } = string.Empty; 
    public List<string> TravelStyle { get; set; } = new(); 
    public string TravelPace { get; set; } = string.Empty; 
    public string Accommodation { get; set; } = string.Empty; 
    public string DaysRhythm { get; set; } = string.Empty; 
    public string OtherNeeds { get; set; } = string.Empty;
}

/// <summary>
/// الرد النهائي الذي يذهب للـ Controller ثم للـ Frontend
/// </summary>
public class TripPlanResponse
{
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string HistoricalBackground { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public TripOverviewData TripOverview { get; set; } = new();
    public AiBudgetAssumption EstimatedCosts { get; set; } = new();
    
    // مصفوفات التوصيات باستخدام الـ DTOs الأصلية للمشروع
    public List<HotelCardWrapper> LodgingRecommendations { get; set; } = new();
    public List<TripCardWrapper> TripRecommendations { get; set; } = new();
    public List<AttractionCardWrapper> AttractionRecommendations { get; set; } = new();
    public List<CarCardWrapper> CarRecommendations { get; set; } = new();
    
    public List<DayItineraryDTO> Itinerary { get; set; } = new();
    public List<string> TravelTips { get; set; } = new();
    public bool Success { get; set; } = true;
    public string? ErrorMessage { get; set; }
}

// --- Wrappers لضمان توافق الـ JSON مع Vue Props ---

public class HotelCardWrapper { public HotelResponseDTO Hotel { get; set; } = new(); }
public class TripCardWrapper { public TripResponseDTO Trip { get; set; } = new(); }
public class CarCardWrapper { public CarResponseDTO Car { get; set; } = new(); }
public class AttractionCardWrapper { public AttractionResponseDTO Attraction { get; set; } = new(); }

// --- كلاسات الميزانية والتفاصيل المساعدة ---

public class AiBudgetAssumption
{
    public decimal TotalPerDay { get; set; }
    public decimal GrandTotal { get; set; }
    public Dictionary<string, decimal> Breakdown { get; set; } = new(); 
    public string Currency { get; set; } = "USD";
}

public class TripOverviewData
{
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public int Travelers { get; set; }
}

public class DayItineraryDTO
{
    public int Day { get; set; }
    public string Date { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<ItineraryActivityDTO> Activities { get; set; } = new();
    public DayMealsDTO Meals { get; set; } = new();
}

public class ItineraryActivityDTO
{
    public string Time { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class DayMealsDTO
{
    public string Breakfast { get; set; } = string.Empty;
    public string Lunch { get; set; } = string.Empty;
    public string Dinner { get; set; } = string.Empty;
}

// ==========================================================
// --- SHADOW DTOS: لاستلام رد الـ AI الخام (الأرقام) ---
// ==========================================================

public class AiRawTripPlanResult
{
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string HistoricalBackground { get; set; } = string.Empty;
    public RawCosts EstimatedCosts { get; set; } = new();
    public List<RawHotelWrapper> LodgingRecommendations { get; set; } = new();
    public List<RawCarWrapper> CarRecommendations { get; set; } = new();
    public List<RawAttractionWrapper> AttractionRecommendations { get; set; } = new();
    public List<RawTripWrapper> TripRecommendations { get; set; } = new();
    public List<DayItineraryDTO> Itinerary { get; set; } = new();
    public List<string> TravelTips { get; set; } = new();

    public class RawCosts { 
        public decimal TotalPerDay { get; set; } 
        public decimal GrandTotal { get; set; } 
        public Dictionary<string, decimal> Breakdown { get; set; } = new(); 
    }
    public class RawHotelWrapper { public RawHotelData Hotel { get; set; } = new(); }
    public class RawHotelData { 
        public string Name { get; set; } = ""; 
        public decimal Price { get; set; } // رقم وليس نص
        public string Location { get; set; } = ""; 
        public double Rating { get; set; } 
        public string Image { get; set; } = ""; 
        public object Reviews { get; set; } = 0; // يقبل رقم أو نص لمنع الـ Exception
        public List<string> Amenities { get; set; } = new();

        public double Latitude { get; set; } 
    public double Longitude { get; set; } 
    }
    
    public class RawCarWrapper { public RawCarData Car { get; set; } = new(); }
    public class RawCarData { 
        public string Name { get; set; } = ""; 
        public decimal Price { get; set; } 
        public string Overview { get; set; } = ""; 
        public List<string> Images { get; set; } = new(); 
        public RawCarReviews Reviews { get; set; } = new();
    }
    public class RawCarReviews { public double OverallRating { get; set; } public int TotalReviews { get; set; } }

    public class RawAttractionWrapper { public RawAttrData Attraction { get; set; } = new(); }
    public class RawAttrData { 
        public int Id { get; set; } 
        public string Title { get; set; } = ""; 
        public decimal Price { get; set; } 
        public string Image { get; set; } = ""; 
        public string Location { get; set; } = ""; 
        public double Rating { get; set; } 
        public object Reviews { get; set; } = 0;

        public double Latitude { get; set; } 
    public double Longitude { get; set; }
    }

    public class RawTripWrapper { public RawTripData Trip { get; set; } = new(); }
    public class RawTripData { 
        public int Id { get; set; } 
        public string Title { get; set; } = ""; 
        public decimal Price { get; set; } 
        public string Image { get; set; } = ""; 
        public string City { get; set; } = ""; 
        public double Rating { get; set; }
        public string Duration { get; set; } = "";
        public string Description { get; set; } = "";
        public RawTripAmenities Amenities { get; set; } = new();
    }
    public class RawTripAmenities { public bool Transport { get; set; } public bool Accommodation { get; set; } public int Meals { get; set; } }
}