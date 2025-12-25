using System.Text.Json;
using System.Text.Json.Serialization;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Agentic_Rentify.Infrastructure.Services;

// Custom converter to handle both string and number price formats  
public class FlexibleStringConverter : JsonConverter<string?>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                return reader.GetString();
            case JsonTokenType.Number:
                if (reader.TryGetInt32(out int intValue))
                    return intValue.ToString();
                if (reader.TryGetDecimal(out decimal decValue))
                    return decValue.ToString();
                return reader.GetDouble().ToString();
            case JsonTokenType.Null:
                return null;
            default:
                throw new JsonException($"Unexpected token type: {reader.TokenType}");
        }
    }

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        if (value == null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value);
    }
}

// Apply converter to all nullable string properties
public class FlexibleStringConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(string);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return new FlexibleStringConverter();
    }
}

public class DataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(ApplicationDbContext context, ILogger<DataSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedDataFromJsonAsync(string jsonFilePath)
    {
        try
        {
            if (!File.Exists(jsonFilePath))
            {
                _logger.LogWarning("db.json file not found at: {Path}", jsonFilePath);
                return;
            }

            var jsonContent = await File.ReadAllTextAsync(jsonFilePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new FlexibleStringConverterFactory() }
            };

            var mockData = JsonSerializer.Deserialize<MockDataRoot>(jsonContent, options);
            if (mockData == null)
            {
                _logger.LogError("Failed to deserialize db.json");
                return;
            }

            // Seed Attractions
            var attractionCount = await _context.Set<Attraction>().CountAsync();
            _logger.LogInformation("Found {Count} existing attractions in database", attractionCount);
            if (attractionCount == 0)
            {
                _logger.LogInformation("Seeding {Count} attractions...", mockData.Attractions?.Count ?? 0);
                if (mockData.Attractions != null && mockData.Attractions.Count > 0)
                {
                    foreach (var attraction in mockData.Attractions)
                    {
                        // Map the JSON data to entity
                        // Skip if essential data is missing
                        if (string.IsNullOrWhiteSpace(attraction.Name) || string.IsNullOrWhiteSpace(attraction.City))
                            continue;

                        var price = ParsePrice(attraction.Price);
                        var latitude = attraction.Latitude ?? 0;
                        var longitude = attraction.Longitude ?? 0;

                        // Ensure no zero prices or coordinates
                        if (price == 0)
                        {
                            _logger.LogWarning("Skipping attraction {Name} - price is 0", attraction.Name);
                            continue;
                        }
                        if (latitude == 0 || longitude == 0)
                        {
                            _logger.LogWarning("Skipping attraction {Name} - invalid coordinates (lat: {Lat}, lng: {Lng})", 
                                attraction.Name, latitude, longitude);
                            continue;
                        }

                        var entity = new Attraction
                        {
                            Name = attraction.Name,
                            City = attraction.City,
                            Rating = attraction.Rating,
                            Price = price,
                            Currency = "$",
                            PaymentId = attraction.PaymentId ?? string.Empty,
                            Status = attraction.Status ?? "Active",
                            Availability = attraction.Availability ?? "Available",
                            IsFeatured = attraction.IsFeatured ?? false,
                            Capacity = attraction.Capacity ?? string.Empty,
                            Description = attraction.Description ?? string.Empty,
                            Overview = attraction.Overview ?? string.Empty,
                            Latitude = latitude,
                            Longitude = longitude,
                            Categories = attraction.Categories ?? new List<string>(),
                            Highlights = attraction.Highlights ?? new List<string>(),
                            Amenities = attraction.Amenities ?? new List<string>(),
                            Images = (attraction.Images ?? new List<string>()).Select(url => new AttractionImage { Url = url }).ToList(),
                            ReviewSummary = new AttractionReviewSummary
                            {
                                OverallRating = attraction.Reviews?.OverallRating ?? 0,
                                TotalReviews = attraction.Reviews?.TotalReviews ?? 0,
                                Criteria = new RatingCriteria
                                {
                                    Experience = attraction.Reviews?.RatingCriteria?.Experience ?? 0,
                                    Staff = attraction.Reviews?.RatingCriteria?.Staff ?? 0,
                                    Accessibility = attraction.Reviews?.RatingCriteria?.Accessibility ?? 0,
                                    Facilities = attraction.Reviews?.RatingCriteria?.Facilities ?? 0,
                                    ValueForMoney = attraction.Reviews?.RatingCriteria?.ValueForMoney ?? 0
                                }
                            },
                            UserReviews = (attraction.UserReviews ?? new List<MockUserReview>()).Select(r => new UserReview
                            {
                                UserName = r.UserName ?? string.Empty,
                                UserImage = r.UserImage ?? string.Empty,
                                Rating = r.Rating,
                                Date = r.Date ?? string.Empty,
                                Comment = r.Comment ?? string.Empty
                            }).ToList()
                        };

                        _context.Set<Attraction>().Add(entity);
                    }
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Attractions seeded successfully");
                }
            }

            // Seed Hotels
            if (!await _context.Set<Hotel>().AnyAsync())
            {
                _logger.LogInformation("Seeding {Count} hotels...", mockData.Hotels?.Count ?? 0);
                if (mockData.Hotels != null && mockData.Hotels.Count > 0)
                {
                    foreach (var hotel in mockData.Hotels)
                    {
                        // Skip if essential data is missing
                        if (string.IsNullOrWhiteSpace(hotel.Name) || string.IsNullOrWhiteSpace(hotel.City))
                            continue;

                        var pricePerNight = ParsePrice(hotel.PricePerNight);
                        var basePrice = pricePerNight > 0 ? pricePerNight : ParsePrice(hotel.BasePrice);
                        var latitude = hotel.Latitude ?? 0;
                        var longitude = hotel.Longitude ?? 0;

                        // Ensure no zero prices or coordinates
                        if (basePrice == 0)
                        {
                            _logger.LogWarning("Skipping hotel {Name} - price is 0", hotel.Name);
                            continue;
                        }
                        if (latitude == 0 || longitude == 0)
                        {
                            _logger.LogWarning("Skipping hotel {Name} - invalid coordinates", hotel.Name);
                            continue;
                        }

                        var entity = new Hotel
                        {
                            Name = hotel.Name,
                            City = hotel.City,
                            Rating = hotel.Rating,
                            ReviewsCount = hotel.ReviewsCount ?? 0,
                            BasePrice = basePrice,
                            PricePerNight = pricePerNight,
                            Description = hotel.Description ?? string.Empty,
                            Overview = hotel.Overview ?? string.Empty,
                            Status = hotel.Status ?? "Active",
                            Featured = hotel.Featured ?? hotel.IsFeatured ?? false,
                            IsFeatured = hotel.IsFeatured ?? hotel.Featured ?? false,
                            Latitude = latitude,
                            Longitude = longitude,
                            Images = hotel.Images ?? new List<string>(),
                            Amenities = hotel.Amenities ?? new List<string>(),
                            RoomAmenities = hotel.RoomAmenities ?? new List<string>(),
                            Highlights = hotel.Highlights ?? new List<string>(),
                            NearbyLocations = hotel.NearbyLocations ?? new List<string>(),
                            Rooms = (hotel.Rooms ?? new List<MockHotelRoom>()).Select(r => new HotelRoom
                            {
                                Type = r.Type ?? string.Empty,
                                Price = ParsePrice(r.Price),
                                Available = r.Available ?? 0
                            }).ToList(),
                            ReviewSummary = new HotelReviewSummary
                            {
                                OverallRating = (hotel.ReviewSummary ?? hotel.Reviews)?.OverallRating ?? 0,
                                TotalReviews = (hotel.ReviewSummary ?? hotel.Reviews)?.TotalReviews ?? 0,
                                RatingCriteria = new RatingCriteria
                                {
                                    Experience = (hotel.ReviewSummary ?? hotel.Reviews)?.RatingCriteria?.Experience ?? 0,
                                    Staff = (hotel.ReviewSummary ?? hotel.Reviews)?.RatingCriteria?.Staff ?? 0,
                                    Accessibility = (hotel.ReviewSummary ?? hotel.Reviews)?.RatingCriteria?.Accessibility ?? 0,
                                    Facilities = (hotel.ReviewSummary ?? hotel.Reviews)?.RatingCriteria?.Facilities ?? 0,
                                    ValueForMoney = (hotel.ReviewSummary ?? hotel.Reviews)?.RatingCriteria?.ValueForMoney ?? 0
                                }
                            },
                            UserReviews = (hotel.UserReviews ?? new List<MockUserReview>()).Select(r => new UserReview
                            {
                                UserName = r.UserName ?? string.Empty,
                                UserImage = r.UserImage ?? string.Empty,
                                Rating = r.Rating,
                                Date = r.Date ?? string.Empty,
                                Comment = r.Comment ?? string.Empty
                            }).ToList(),
                            Availability = new HotelAvailability
                            {
                                AvailableRooms = hotel.Availability?.AvailableRooms ?? 0,
                                TotalRooms = hotel.Availability?.TotalRooms ?? 0
                            }
                        };

                        _context.Set<Hotel>().Add(entity);
                    }
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Hotels seeded successfully");
                }
            }

            // Seed Cars
            if (!await _context.Set<Car>().AnyAsync())
            {
                _logger.LogInformation("Seeding {Count} cars...", mockData.Cars?.Count ?? 0);
                if (mockData.Cars != null && mockData.Cars.Count > 0)
                {
                    foreach (var car in mockData.Cars)
                    {
                        // Skip if essential data is missing
                        if (string.IsNullOrWhiteSpace(car.Name) || string.IsNullOrWhiteSpace(car.City))
                            continue;

                        var price = ParsePrice(car.Price);
                        var basePrice = price > 0 ? price : ParsePrice(car.BasePrice);

                        // Ensure no zero prices
                        if (basePrice == 0)
                        {
                            _logger.LogWarning("Skipping car {Name} - price is 0", car.Name);
                            continue;
                        }


                        var entity = new Car
                        {
                            Name = car.Name ?? string.Empty,
                            Brand = car.Brand ?? string.Empty,
                            Model = car.Model ?? string.Empty,
                            Year = car.Year ?? DateTime.Now.Year,
                            City = car.City ?? string.Empty,
                            Description = car.Description ?? string.Empty,
                            Overview = car.Overview ?? string.Empty,
                            Type = car.Type ?? string.Empty,
                            Seats = car.Seats ?? 4,
                            Transmission = car.Transmission ?? "automatic",
                            FuelType = car.FuelType ?? "Petrol",
                            Price = ParsePrice(car.Price),
                            BasePrice = ParsePrice(car.BasePrice),
                            TotalFleet = car.TotalFleet ?? 0,
                            AvailableNow = car.AvailableNow ?? 0,
                            NextAvailability = car.NextAvailability ?? string.Empty,
                            Status = car.Status ?? "Available",
                            Featured = car.Featured ?? false,
                            IsFeatured = car.IsFeatured ?? false,
                            Features = car.Features ?? new List<string>(),
                            Images = car.Images ?? new List<string>(),
                            Amenities = car.Amenities ?? new List<string>(),
                            ReviewSummary = new CarReviewSummary
                            {
                                OverallRating = car.ReviewSummary?.OverallRating ?? 0,
                                TotalReviews = car.ReviewSummary?.TotalReviews ?? 0,
                                RatingCriteria = new CarRatingCriteria
                                {
                                    Experience = car.ReviewSummary?.RatingCriteria?.Experience ?? 0,
                                    Comfort = car.ReviewSummary?.RatingCriteria?.Comfort ?? 0,
                                    Reliability = car.ReviewSummary?.RatingCriteria?.Reliability ?? 0,
                                    ValueForMoney = car.ReviewSummary?.RatingCriteria?.ValueForMoney ?? 0,
                                    Features = car.ReviewSummary?.RatingCriteria?.Features ?? 0
                                }
                            },
                            UserReviews = (car.UserReviews ?? new List<MockUserReview>()).Select(r => new UserReview
                            {
                                UserName = r.UserName ?? string.Empty,
                                UserImage = r.UserImage ?? string.Empty,
                                Rating = r.Rating,
                                Date = r.Date ?? string.Empty,
                                Comment = r.Comment ?? string.Empty
                            }).ToList()
                        };

                        _context.Set<Car>().Add(entity);
                    }
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Cars seeded successfully");
                }
            }

            // Seed Trips
            if (!await _context.Set<Trip>().AnyAsync())
            {
                _logger.LogInformation("Seeding {Count} trips...", mockData.Trips?.Count ?? 0);
                if (mockData.Trips != null && mockData.Trips.Count > 0)
                {
                    foreach (var trip in mockData.Trips)
                    {
                        // Skip if essential data is missing
                        if (string.IsNullOrWhiteSpace(trip.Title) || string.IsNullOrWhiteSpace(trip.City))
                            continue;

                        var price = ParsePrice(trip.Price);

                        // Ensure no zero prices
                        if (price == 0)
                        {
                            _logger.LogWarning("Skipping trip {Title} - price is 0", trip.Title);
                            continue;
                        }

                        var tripEntity = new Trip
                        {
                            Title = trip.Title,
                            Description = trip.Description ?? string.Empty,
                            City = trip.City,
                            Destination = trip.Destination ?? trip.City,
                            TripType = trip.TripType ?? string.Empty,
                            Price = price,
                            Rating = trip.Rating,
                            TotalReviews = trip.TotalReviews ?? trip.Reviews ?? 0,
                            Reviews = trip.Reviews ?? trip.TotalReviews ?? 0,
                            Duration = trip.Duration ?? string.Empty,
                            MainImage = trip.MainImage ?? trip.Image ?? string.Empty,
                            MaxPeople = trip.MaxPeople ?? 20,
                            AvailableSpots = trip.AvailableSpots ?? trip.MaxPeople ?? 0,
                            Status = trip.Status ?? "Ongoing",
                            Featured = trip.Featured ?? trip.IsFeatured ?? false,
                            IsFeatured = trip.IsFeatured ?? trip.Featured ?? false,
                            Images = trip.Images ?? new List<string>(),
                            AvailableDates = trip.AvailableDates ?? new List<string>(),
                            Highlights = trip.Highlights ?? new List<string>(),
                            Itinerary = (trip.Itinerary ?? new List<MockItineraryDay>()).Select(day => new ItineraryDay
                            {
                                Day = day.Day,
                                Title = day.Title ?? string.Empty,
                                Description = day.Description ?? string.Empty,
                                Activities = (day.Activities ?? new List<MockActivity>()).Select(a => new Activity
                                {
                                    Time = a.Time ?? string.Empty,
                                    Title = a.Title ?? string.Empty,
                                    Desc = a.Description ?? a.Desc ?? string.Empty
                                }).ToList()
                            }).ToList(),
                            ReviewSummary = new TripReviewSummary
                            {
                                OverallRating = trip.ReviewSummary?.OverallRating ?? 0,
                                TotalReviews = trip.ReviewSummary?.TotalReviews ?? 0,
                                RatingCriteria = new RatingCriteria
                                {
                                    Experience = trip.ReviewSummary?.RatingCriteria?.Experience ?? 0,
                                    Staff = trip.ReviewSummary?.RatingCriteria?.Staff ?? 0,
                                    Accessibility = trip.ReviewSummary?.RatingCriteria?.Accessibility ?? 0,
                                    Facilities = trip.ReviewSummary?.RatingCriteria?.Facilities ?? 0,
                                    ValueForMoney = trip.ReviewSummary?.RatingCriteria?.ValueForMoney ?? 0
                                }
                            },
                            UserReviews = (trip.UserReviews ?? new List<MockUserReview>()).Select(r => new UserReview
                            {
                                UserName = r.UserName ?? r.User ?? string.Empty,
                                UserImage = r.UserImage ?? string.Empty,
                                Rating = r.Rating,
                                Date = r.Date ?? string.Empty,
                                Comment = r.Comment ?? r.Text ?? string.Empty
                            }).ToList(),
                            Amenities = new TripAmenitiesInfo
                            {
                                Transport = trip.Amenities?.Transport ?? false,
                                Accommodation = trip.Amenities?.Accommodation ?? false,
                                Meals = trip.Amenities?.Meals ?? 0
                            },
                            HotelInfo = (trip.HotelInfo ?? trip.Hotel) != null ? new TripHotelInfo
                            {
                                Name = (trip.HotelInfo ?? trip.Hotel)!.Name ?? string.Empty,
                                Rating = (trip.HotelInfo ?? trip.Hotel)!.Rating,
                                Image = (trip.HotelInfo ?? trip.Hotel)!.Image ?? string.Empty,
                                Features = (trip.HotelInfo ?? trip.Hotel)!.Features ?? new List<string>()
                            } : null
                        };

                        _context.Set<Trip>().Add(tripEntity);
                    }
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Trips seeded successfully");
                }
            }

            _logger.LogInformation("All data seeding completed successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during data seeding from JSON");
            throw;
        }
    }

    // Helper method to parse price strings like "12$", "150 EGP", "3400 $" to decimal
    private static decimal ParsePrice(string? priceString)
    {
        if (string.IsNullOrWhiteSpace(priceString)) return 0;
        
        // Remove currency symbols and extract numbers
        var numericPart = new string(priceString.Where(c => char.IsDigit(c) || c == '.').ToArray());
        return decimal.TryParse(numericPart, out var price) ? price : 0;
    }
}

// Mock data DTOs for JSON deserialization
public class MockDataRoot
{
    public List<MockAttraction>? Attractions { get; set; }
    public List<MockHotel>? Hotels { get; set; }
    public List<MockCar>? Cars { get; set; }
    public List<MockTrip>? Trips { get; set; }
}

public class MockAttraction
{
    public string? Name { get; set; }
    public string? City { get; set; }
    public double Rating { get; set; }
    public string? Price { get; set; }
    public string? PaymentId { get; set; }
    public string? Status { get; set; }
    public string? Availability { get; set; }
    public bool? IsFeatured { get; set; }
    public string? Capacity { get; set; }
    public string? Description { get; set; }
    public string? Overview { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public List<string>? Categories { get; set; }
    public List<string>? Highlights { get; set; }
    public List<string>? Amenities { get; set; }
    public List<string>? Images { get; set; }
    public MockReviewSummary? Reviews { get; set; }
    public List<MockUserReview>? UserReviews { get; set; }
}

public class MockHotel
{
    public string? Name { get; set; }
    public string? City { get; set; }
    public double Rating { get; set; }
    public int? ReviewsCount { get; set; }
    public string? BasePrice { get; set; }
    public string? PricePerNight { get; set; }
    public string? Description { get; set; }
    public string? Overview { get; set; }
    public string? Status { get; set; }
    public bool? Featured { get; set; }
    public bool? IsFeatured { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public List<string>? Images { get; set; }
    public List<string>? Amenities { get; set; }
    public List<string>? RoomAmenities { get; set; }
    public List<string>? Highlights { get; set; }
    public List<string>? NearbyLocations { get; set; }
    public List<MockHotelRoom>? Rooms { get; set; }
    public MockReviewSummary? Reviews { get; set; }
    public MockReviewSummary? ReviewSummary { get; set; }
    public List<MockUserReview>? UserReviews { get; set; }
    public MockHotelAvailability? Availability { get; set; }
}

public class MockCar
{
    public string? Name { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
    public string? City { get; set; }
    public string? Description { get; set; }
    public string? Overview { get; set; }
    public string? Type { get; set; }
    public int? Seats { get; set; }
    public string? Transmission { get; set; }
    public string? FuelType { get; set; }
    public string? Price { get; set; }
    public string? BasePrice { get; set; }
    public int? Total_fleet { get; set; }
    public int? TotalFleet { get; set; }
    public int? Available_now { get; set; }
    public int? AvailableNow { get; set; }
    public string? NextAvailability { get; set; }
    public string? Status { get; set; }
    public bool? Featured { get; set; }
    public bool? IsFeatured { get; set; }
    public List<string>? Features { get; set; }
    public List<string>? Images { get; set; }
    public List<string>? Amenities { get; set; }
    public MockCarReviewSummary? Reviews { get; set; }
    public MockCarReviewSummary? ReviewSummary { get; set; }
    public List<MockUserReview>? UserReviews { get; set; }
}

public class MockTrip
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? City { get; set; }
    public string? Destination { get; set; }
    public string? TripType { get; set; }
    public string? Price { get; set; }
    public double Rating { get; set; }
    public int? TotalReviews { get; set; }
    public int? Reviews { get; set; }
    public string? Duration { get; set; }
    public string? Image { get; set; }
    public string? MainImage { get; set; }
    public int? MaxPeople { get; set; }
    public int? AvailableSpots { get; set; }
    public string? Status { get; set; }
    public string? Availability { get; set; }
    public bool? Featured { get; set; }
    public bool? IsFeatured { get; set; }
    public List<string>? Images { get; set; }
    public List<string>? AvailableDates { get; set; }
    public List<string>? Highlights { get; set; }
    public List<MockItineraryDay>? Itinerary { get; set; }
    public MockReviewSummary? ReviewSummary { get; set; }
    public List<MockUserReview>? UserReviews { get; set; }
    public MockTripAmenities? Amenities { get; set; }
    public MockTripHotelInfo? Hotel { get; set; }
    public MockTripHotelInfo? HotelInfo { get; set; }
}

public class MockCoordinates
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}

public class MockReviewSummary
{
    public double OverallRating { get; set; }
    public int TotalReviews { get; set; }
    public MockRatingCriteria? RatingCriteria { get; set; }
}

public class MockCarReviewSummary
{
    public double OverallRating { get; set; }
    public int TotalReviews { get; set; }
    public MockCarRatingCriteria? RatingCriteria { get; set; }
}

public class MockRatingCriteria
{
    public double Experience { get; set; }
    public double Staff { get; set; }
    public double Accessibility { get; set; }
    public double Facilities { get; set; }
    public double ValueForMoney { get; set; }
}

public class MockCarRatingCriteria
{
    public double Experience { get; set; }
    public double Comfort { get; set; }
    public double Reliability { get; set; }
    public double ValueForMoney { get; set; }
    public double Features { get; set; }
}

public class MockUserReview
{
    public string? UserName { get; set; }
    public string? User { get; set; }
    public string? UserImage { get; set; }
    public double Rating { get; set; }
    public string? Date { get; set; }
    public string? Comment { get; set; }
    public string? Text { get; set; }
}

public class MockHotelRoom
{
    public string? Type { get; set; }
    public string? Price { get; set; }
    public int? Available { get; set; }
}

public class MockHotelAvailability
{
    public int AvailableRooms { get; set; }
    public int TotalRooms { get; set; }
}

public class MockItineraryDay
{
    public int Day { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public List<MockActivity>? Activities { get; set; }
}

public class MockActivity
{
    public string? Time { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Desc { get; set; }
}

public class MockTripAmenities
{
    public bool Transport { get; set; }
    public bool Accommodation { get; set; }
    public int Meals { get; set; }
}

public class MockTripHotelInfo
{
    public string? Name { get; set; }
    public double Rating { get; set; }
    public string? Image { get; set; }
    public List<string>? Features { get; set; }
}
