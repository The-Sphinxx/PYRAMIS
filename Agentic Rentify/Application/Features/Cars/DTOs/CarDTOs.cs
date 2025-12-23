namespace Agentic_Rentify.Application.Features.Cars.DTOs;

public class CarResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string City { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Seats { get; set; }
    public string Transmission { get; set; } = "automatic";
    public string FuelType { get; set; } = "Petrol";
    public string Price { get; set; } = string.Empty; // Formatted
    public int TotalFleet { get; set; }
    public int AvailableNow { get; set; }
    public string NextAvailability { get; set; } = string.Empty;
    public string Status { get; set; } = "Available";
    public bool Featured { get; set; } = false;
    public bool IsFeatured { get; set; } = false;
    public List<string> Features { get; set; } = new();
    public List<string> Images { get; set; } = new();
    public List<string> Amenities { get; set; } = new();
    public CarReviewSummaryDTO ReviewSummary { get; set; } = new();
    public List<UserReviewDTO> UserReviews { get; set; } = new();
}

public class CarReviewSummaryDTO
{
    public double OverallRating { get; set; }
    public int TotalReviews { get; set; }
    public CarRatingCriteriaDTO RatingCriteria { get; set; } = new();
}

public class CarRatingCriteriaDTO
{
    public double Experience { get; set; }
    public double Comfort { get; set; }
    public double Reliability { get; set; }
    public double ValueForMoney { get; set; }
    public double Features { get; set; }
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

public class CreateCarDTO
{
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string City { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Seats { get; set; }
    public string Transmission { get; set; } = "automatic";
    public string FuelType { get; set; } = "Petrol";
    public decimal Price { get; set; }
    public string Overview { get; set; } = string.Empty;
    public List<string> Features { get; set; } = new();
    public List<string> Images { get; set; } = new();
    public List<string> Amenities { get; set; } = new();
}

public class UpdateCarDTO : CreateCarDTO
{
    public int Id { get; set; }
}
