using System.ComponentModel;
using System.Text.Json;
using Agentic_Rentify.Application.Features.Hotels.Queries.GetAllHotels;
using Agentic_Rentify.Application.Features.Attractions.Queries.GetAllAttractions;
using Agentic_Rentify.Application.Features.Cars.Queries.GetAllCars;
using Agentic_Rentify.Application.Features.Trips.Queries.GetAllTrips;
using Agentic_Rentify.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

namespace Agentic_Rentify.Infragentic.Plugins;

/// <summary>
/// Smart Brain (Grok) Plugin - Stage 1: Data Fetching
/// Fetches real SQL data using CQRS/Specifications and returns full DTO objects (the "Ingredients")
/// </summary>
public class DataFetchPlugin(IServiceScopeFactory serviceScopeFactory)
{
    [KernelFunction("fetch_hotels_for_planner")]
    [Description("CRITICAL: Fetches COMPLETE hotel DTOs with ALL fields (id, name, city, rating, reviewsCount, pricePerNight, amenities, images, latitude, longitude, etc.) for trip planning. Returns full objects, NOT just names.")]
    public async Task<string> FetchHotelsForPlannerAsync(
        [Description("Destination city (e.g., 'Cairo', 'Luxor', 'Sharm El Sheikh')")] string destination,
        [Description("Budget level: 'budget' (<$100/night), 'mid-range' ($100-$250), 'luxury' (>$250)")] string budgetLevel = "mid-range",
        [Description("Maximum number of hotels to return")] int topK = 5)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var vectorService = scope.ServiceProvider.GetRequiredService<IVectorDbService>();

        // Step 1: Use vector search to find relevant hotel IDs
        const string collection = "rentify_memory";
        await vectorService.CreateCollectionIfNotExists(collection);
        
        var searchQuery = $"hotels in {destination} suitable for {budgetLevel} travelers";
        var vectorResults = await vectorService.SearchByTextAsync(collection, searchQuery, topK * 2);
        
        // Step 2: Fetch FULL hotel DTOs using MediatR
        var query = new GetAllHotelsQuery
        {
            SearchTerm = destination,
            PageSize = topK * 2,
            PageNumber = 1
        };

        var result = await mediator.Send(query);
        
        // Step 3: Filter by budget level and ensure valid coordinates
        var filteredHotels = FilterHotelsByBudget(result.Data, budgetLevel)
            .Where(h => h.Latitude != 0 && h.Longitude != 0) // CRITICAL: Skip hotels with invalid coordinates
            .Take(topK)
            .ToList();

        if (!filteredHotels.Any())
        {
            return JsonSerializer.Serialize(new 
            { 
                success = false,
                message = $"No hotels found in {destination} for {budgetLevel} budget",
                hotels = Array.Empty<object>(),
                alternative_suggestion = GetAlternativeDestination(destination)
            });
        }

        // Step 4: Return COMPLETE DTO objects (the "Ingredients")
        return JsonSerializer.Serialize(new
        {
            success = true,
            destination,
            budgetLevel,
            count = filteredHotels.Count,
            hotels = filteredHotels.Select(h => new
            {
                // CRITICAL: All fields for HotelCard.vue props
                id = h.Id,
                name = h.Name,
                city = h.City,
                image = (h.Images != null && h.Images.Count > 0) ? h.Images[0] : "/images/hotels/default.jpg",
                price = ExtractNumericPrice(h.PricePerNight),
                location = h.City,
                rating = h.Rating,
                reviews = h.ReviewsCount.ToString(),
                amenities = h.Amenities ?? new List<string>(),
                
                // Additional metadata for planning
                latitude = h.Latitude,
                longitude = h.Longitude,
                images = h.Images ?? new List<string>(),
                description = h.Description,
                reviewsCount = h.ReviewsCount,
                facilities = h.Amenities
            })
        });
    }

    [KernelFunction("fetch_attractions_for_planner")]
    [Description("CRITICAL: Fetches COMPLETE attraction DTOs with ALL fields (id, title, location, price, rating, reviews, image, latitude, longitude) for trip planning.")]
    public async Task<string> FetchAttractionsForPlannerAsync(
        [Description("Destination city")] string destination,
        [Description("User interests (e.g., 'museums', 'adventure', 'historical sites')")] string? interests = null,
        [Description("Maximum number of attractions")] int topK = 10)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var vectorService = scope.ServiceProvider.GetRequiredService<IVectorDbService>();

        const string collection = "rentify_memory";
        await vectorService.CreateCollectionIfNotExists(collection);
        
        var searchQuery = string.IsNullOrEmpty(interests)
            ? $"attractions and places to visit in {destination}"
            : $"{interests} attractions and activities in {destination}";
            
        var vectorResults = await vectorService.SearchByTextAsync(collection, searchQuery, topK * 2);
        
        var query = new GetAllAttractionsQuery
        {
            City = destination,
            PageSize = topK * 2,
            PageNumber = 1
        };

        var result = await mediator.Send(query);

        if (!result.Data.Any())
        {
            return JsonSerializer.Serialize(new 
            { 
                success = false,
                message = $"No attractions found in {destination}",
                attractions = Array.Empty<object>()
            });
        }

        // Filter: Only include attractions with valid coordinates
        var attractions = result.Data
            .Where(a => a.Latitude != 0 && a.Longitude != 0)
            .Take(topK)
            .ToList();

        if (!attractions.Any())
        {
            return JsonSerializer.Serialize(new 
            { 
                success = false,
                message = $"No attractions with valid coordinates found in {destination}",
                attractions = Array.Empty<object>()
            });
        }

        return JsonSerializer.Serialize(new
        {
            success = true,
            destination,
            interests,
            count = attractions.Count,
            attractions = attractions.Select(a => new
            {
                // CRITICAL: All fields for AttractionCard.vue props
                id = a.Id,
                title = a.Name,
                image = (a.Images != null && a.Images.Count > 0) ? a.Images[0] : "/images/attractions/default.jpg",
                price = ExtractNumericPrice(a.Price),
                location = a.City,
                rating = a.Rating,
                reviews = a.Reviews?.TotalReviews ?? 0,
                
                // Additional metadata
                latitude = a.Latitude,
                longitude = a.Longitude,
                images = a.Images ?? new List<string>(),
                categories = a.Categories ?? new List<string>(),
                description = a.Description,
                amenities = a.Amenities ?? new List<string>()
            })
        });
    }

    [KernelFunction("fetch_vehicles_for_planner")]
    [Description("CRITICAL: Fetches COMPLETE car DTOs with ALL fields (id, name, overview, images, price, reviews) for trip planning.")]
    public async Task<string> FetchVehiclesForPlannerAsync(
        [Description("Destination city")] string destination,
        [Description("Number of travelers (to match seat capacity)")] int travelers = 2,
        [Description("Budget level: 'budget', 'mid-range', 'luxury'")] string budgetLevel = "mid-range",
        [Description("Maximum number of vehicles")] int topK = 5)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var query = new GetAllCarsQuery
        {
            SearchTerm = destination,
            PageSize = topK * 2,
            PageNumber = 1
        };

        var result = await mediator.Send(query);
        
        // Filter by destination (city) first, then by capacity and budget
        var filteredCars = result.Data
            .Where(c => c.City != null && string.Equals(c.City.ToString().Trim(), destination, StringComparison.OrdinalIgnoreCase))
            .Cast<dynamic>()
            .ToList();
            
        filteredCars = FilterCarsByCapacityAndBudget(filteredCars, travelers, budgetLevel)
            .Take(topK)
            .ToList();

        if (!filteredCars.Any())
        {
            return JsonSerializer.Serialize(new 
            { 
                success = false,
                message = $"No vehicles found for {travelers} travelers at {budgetLevel} budget",
                vehicles = Array.Empty<object>()
            });
        }

        return JsonSerializer.Serialize(new
        {
            success = true,
            destination,
            travelers,
            budgetLevel,
            count = filteredCars.Count,
            vehicles = filteredCars.Select(c => new
            {
                // CRITICAL: All fields for CarCard.vue props
                id = c.Id,
                name = c.Name,
                overview = c.Overview,
                images = c.Images ?? new List<string>(),
                price = ExtractNumericPriceAny(c.Price),
                reviews = new
                {
                    overallRating = c.ReviewSummary?.OverallRating ?? 0,
                    totalReviews = c.ReviewSummary?.TotalReviews ?? 0
                },
                
                // Additional metadata
                type = c.Type,
                seats = c.Seats,
                transmission = c.Transmission,
                fuelType = c.FuelType,
                features = c.Features ?? new List<string>(),
                brand = c.Brand,
                model = c.Model
            })
        });
    }

    // Helper Methods
    private static IEnumerable<dynamic> FilterHotelsByBudget(IEnumerable<dynamic> hotels, string budgetLevel)
    {
        return budgetLevel.ToLower() switch
        {
            "budget" => hotels.Where(h => ExtractNumericPrice(h.PricePerNight) < 100),
            "luxury" => hotels.Where(h => ExtractNumericPrice(h.PricePerNight) > 250),
            _ => hotels.Where(h => 
            {
                var price = ExtractNumericPrice(h.PricePerNight);
                return price >= 100 && price <= 250;
            })
        };
    }

    private static IEnumerable<dynamic> FilterCarsByCapacityAndBudget(
        IEnumerable<dynamic> cars, 
        int travelers, 
        string budgetLevel)
    {
        var filtered = cars.Where(c => c.Seats >= travelers);
        
        return budgetLevel.ToLower() switch
        {
            "budget" => filtered.Where(c => ExtractNumericPriceAny(c.Price) < 50),
            "luxury" => filtered.Where(c => ExtractNumericPriceAny(c.Price) > 150),
            _ => filtered.Where(c => 
            {
                var price = ExtractNumericPriceAny(c.Price);
                return price >= 50 && price <= 150;
            })
        };
    }

    private static decimal ExtractNumericPrice(string priceString)
    {
        if (string.IsNullOrEmpty(priceString))
            return 0;

        var numericOnly = new string(priceString.Where(c => char.IsDigit(c) || c == '.').ToArray());
        return decimal.TryParse(numericOnly, out var price) ? price : 0;
    }

    private static decimal ExtractNumericPriceAny(object priceObj)
    {
        if (priceObj == null) return 0;
        try
        {
            switch (priceObj)
            {
                case decimal dec: return dec;
                case double dbl: return (decimal)dbl;
                case float flt: return (decimal)flt;
                case int i: return i;
                case long l: return l;
                case string s: return ExtractNumericPrice(s);
                default:
                    var s2 = priceObj.ToString();
                    return string.IsNullOrWhiteSpace(s2) ? 0 : ExtractNumericPrice(s2);
            }
        }
        catch
        {
            return 0;
        }
    }

    private static string GetAlternativeDestination(string original)
    {
        var alternatives = new Dictionary<string, string>
        {
            { "Cairo", "Giza or Alexandria" },
            { "Luxor", "Aswan or Cairo" },
            { "Sharm El Sheikh", "Hurghada or Dahab" },
            { "Hurghada", "Sharm El Sheikh or Marsa Alam" }
        };

        return alternatives.TryGetValue(original, out var alt) 
            ? alt 
            : "Please try a major Egyptian city like Cairo, Luxor, or Sharm El Sheikh";
    }
}
