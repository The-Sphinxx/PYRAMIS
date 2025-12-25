using Agentic_Rentify.Core.Entities;

namespace Agentic_Rentify.Application.Features.Hotels.DTOs;

public class HotelResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public double Rating { get; set; }
    public int ReviewsCount { get; set; }
    public string BasePrice { get; set; } = string.Empty; // Formatted
    public decimal RawBasePrice { get; set; }
    public string PricePerNight { get; set; } = string.Empty;
    public decimal RawPricePerNight { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public bool Featured { get; set; } = false;
    public bool IsFeatured { get; set; } = false;
    public List<string> Images { get; set; } = new();
    public List<string> Amenities { get; set; } = new();
    public List<string> RoomAmenities { get; set; } = new();
    public List<string> Highlights { get; set; } = new();
    public List<string> NearbyLocations { get; set; } = new();
    public List<string> Facilities { get; set; } = new();
    public List<HotelRoomDTO> Rooms { get; set; } = new();
    public HotelReviewSummaryDTO ReviewSummary { get; set; } = new();
    public List<UserReviewDTO> UserReviews { get; set; } = new();
    public HotelAvailabilityDTO Availability { get; set; } = new();
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class HotelReviewSummaryDTO
{
    public double OverallRating { get; set; }
    public int TotalReviews { get; set; }
    public RatingCriteriaDTO RatingCriteria { get; set; } = new();
}

public class RatingCriteriaDTO
{
    public double Experience { get; set; }
    public double Staff { get; set; }
    public double Accessibility { get; set; }
    public double Facilities { get; set; }
    public double ValueForMoney { get; set; }
}

public class UserReviewDTO
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserImage { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string Date { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
}

public class HotelAvailabilityDTO
{
    public int AvailableRooms { get; set; }
    public int TotalRooms { get; set; }
}

public class HotelRoomDTO
{
    public string Type { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool Available { get; set; }
}

public class CreateHotelDTO
{
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal PricePerNight { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<string> Images { get; set; } = new();
    public List<string> Facilities { get; set; } = new();
    public List<HotelRoomDTO> Rooms { get; set; } = new();
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class UpdateHotelDTO : CreateHotelDTO
{
    public int Id { get; set; }
}

public class PatchHotelDTO
{
    public string? Status { get; set; }
    public bool? Featured { get; set; }
    public bool? IsFeatured { get; set; }
}
