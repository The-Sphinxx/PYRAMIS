using System.ComponentModel;
using System.Text.Json;
using Agentic_Rentify.Application.Features.Hotels.Queries.GetAllHotels;
using Agentic_Rentify.Application.Features.Attractions.Queries.GetAllAttractions;
using Agentic_Rentify.Application.Features.Cars.Queries.GetAllCars;
using Agentic_Rentify.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

namespace Agentic_Rentify.Infragentic.Plugins;

/// <summary>
/// Plugin for retrieving real travel data from Qdrant vector database
/// using semantic search based on user criteria (destination, budget).
/// </summary>
public class TravelDatabasePlugin(IServiceScopeFactory serviceScopeFactory)
{
    [KernelFunction("search_hotels_by_destination")]
    [Description("Searches for hotels in a specific city/destination using vector similarity. Returns real hotels with coordinates, amenities, and pricing information.")]
    public async Task<string> SearchHotelsByDestinationAsync(
        [Description("The destination city or location to search for hotels (e.g., 'Cairo', 'Luxor', 'Sharm El Sheikh')")] string destination,
        [Description("Budget level: 'budget', 'mid-range', 'luxury'. Used to filter hotels by price range.")] string budgetLevel = "mid-range",
        [Description("Maximum number of results to return")] int topK = 5)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var vectorService = scope.ServiceProvider.GetRequiredService<IVectorDbService>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        // Perform semantic search for hotels in the destination
        const string collection = "rentify_memory";
        await vectorService.CreateCollectionIfNotExists(collection);
        
        var searchQuery = $"hotels in {destination} suitable for {budgetLevel} travelers";
        var vectorResults = await vectorService.SearchByTextAsync(collection, searchQuery, topK * 2);
        
        // Filter only hotel entities
        var hotelIds = vectorResults
            .Where(r => r.Type.Equals("hotel", StringComparison.OrdinalIgnoreCase))
            .Select(r => int.Parse(r.EntityId))
            .Take(topK)
            .ToList();

        if (!hotelIds.Any())
        {
            return JsonSerializer.Serialize(new { message = "No hotels found for this destination", hotels = Array.Empty<object>() });
        }

        // Fetch full hotel details
        var query = new GetAllHotelsQuery
        {
            SearchTerm = destination,
            PageSize = topK,
            PageNumber = 1
        };

        var result = await mediator.Send(query);
        
        // Filter by budget level
        var filteredHotels = FilterHotelsByBudget(result.Data, budgetLevel);

        return JsonSerializer.Serialize(new
        {
            destination,
            budgetLevel,
            count = filteredHotels.Count(),
            hotels = filteredHotels.Select(h => new
            {
                h.Id,
                h.Name,
                h.City,
                h.Rating,
                h.PricePerNight,
                h.Description,
                h.Latitude,
                h.Longitude,
                h.Amenities,
                h.Images,
                h.ReviewsCount
            })
        });
    }

    [KernelFunction("search_attractions_by_destination")]
    [Description("Searches for tourist attractions, landmarks, and activities in a specific destination using vector similarity.")]
    public async Task<string> SearchAttractionsByDestinationAsync(
        [Description("The destination city or location to search for attractions")] string destination,
        [Description("Optional: specific interests or categories (e.g., 'museums', 'adventure', 'historical sites')")] string? interests = null,
        [Description("Maximum number of results to return")] int topK = 8)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var vectorService = scope.ServiceProvider.GetRequiredService<IVectorDbService>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        const string collection = "rentify_memory";
        await vectorService.CreateCollectionIfNotExists(collection);
        
        var searchQuery = string.IsNullOrEmpty(interests)
            ? $"attractions and places to visit in {destination}"
            : $"{interests} attractions and activities in {destination}";
            
        var vectorResults = await vectorService.SearchByTextAsync(collection, searchQuery, topK * 2);
        
        // Filter only attraction entities
        var attractionIds = vectorResults
            .Where(r => r.Type.Equals("attraction", StringComparison.OrdinalIgnoreCase))
            .Select(r => int.Parse(r.EntityId))
            .Take(topK)
            .ToList();

        if (!attractionIds.Any())
        {
            return JsonSerializer.Serialize(new { message = "No attractions found for this destination", attractions = Array.Empty<object>() });
        }

        // Fetch full attraction details
        var query = new GetAllAttractionsQuery
        {
            City = destination,
            PageSize = topK,
            PageNumber = 1
        };

        var result = await mediator.Send(query);

        return JsonSerializer.Serialize(new
        {
            destination,
            interests,
            count = result.Data.Count(),
            attractions = result.Data.Select(a => new
            {
                a.Id,
                a.Name,
                a.City,
                a.Rating,
                a.Price,
                a.Description,
                a.Latitude,
                a.Longitude,
                a.Categories,
                a.Amenities,
                a.Images
            })
        });
    }

    [KernelFunction("search_vehicles_by_criteria")]
    [Description("Searches for rental vehicles (cars) based on destination and budget level. Returns real vehicles with specifications and pricing.")]
    public async Task<string> SearchVehiclesByCriteriaAsync(
        [Description("The city where the vehicle will be rented")] string city,
        [Description("Budget level: 'budget', 'mid-range', 'luxury'")] string budgetLevel = "mid-range",
        [Description("Optional: number of seats required")] int? minSeats = null,
        [Description("Maximum number of results to return")] int topK = 5)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var vectorService = scope.ServiceProvider.GetRequiredService<IVectorDbService>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        const string collection = "rentify_memory";
        await vectorService.CreateCollectionIfNotExists(collection);
        
        var searchQuery = $"rental cars in {city} for {budgetLevel} budget travelers";
        var vectorResults = await vectorService.SearchByTextAsync(collection, searchQuery, topK * 2);
        
        // Filter only car entities
        var carIds = vectorResults
            .Where(r => r.Type.Equals("car", StringComparison.OrdinalIgnoreCase) || r.Type.Equals("vehicle", StringComparison.OrdinalIgnoreCase))
            .Select(r => int.Parse(r.EntityId))
            .Take(topK)
            .ToList();

        // Fetch full car details
        var query = new GetAllCarsQuery
        {
            SearchTerm = city,
            PageSize = topK,
            PageNumber = 1
        };

        var result = await mediator.Send(query);
        
        // Filter by budget and seats
        var filteredCars = FilterCarsByBudgetAndSeats(result.Data, budgetLevel, minSeats);

        return JsonSerializer.Serialize(new
        {
            city,
            budgetLevel,
            minSeats,
            count = filteredCars.Count(),
            vehicles = filteredCars.Select(c => new
            {
                c.Id,
                c.Name,
                c.Brand,
                c.Model,
                c.Type,
                c.Seats,
                c.Transmission,
                c.FuelType,
                c.Price,
                c.Features,
                c.Images,
                c.City
            })
        });
    }

    [KernelFunction("get_budget_ranges")]
    [Description("Returns the price ranges for different budget levels to help filter accommodations and vehicles.")]
    public Task<string> GetBudgetRangesAsync()
    {
        var budgetRanges = new
        {
            hotels = new
            {
                budget = new { min = 0, max = 100, description = "Budget hotels: $0-100 per night" },
                midRange = new { min = 100, max = 300, description = "Mid-range hotels: $100-300 per night" },
                luxury = new { min = 300, max = 10000, description = "Luxury hotels: $300+ per night" }
            },
            vehicles = new
            {
                budget = new { min = 0, max = 50, description = "Budget cars: $0-50 per day" },
                midRange = new { min = 50, max = 150, description = "Mid-range cars: $50-150 per day" },
                luxury = new { min = 150, max = 10000, description = "Luxury cars: $150+ per day" }
            }
        };

        return Task.FromResult(JsonSerializer.Serialize(budgetRanges));
    }

    // Helper methods for filtering
    private static IEnumerable<dynamic> FilterHotelsByBudget(IEnumerable<dynamic> hotels, string budgetLevel)
    {
        return budgetLevel.ToLower() switch
        {
            "budget" => hotels.Where(h => h.PricePerNight <= 100),
            "mid-range" or "mid_range" or "midrange" => hotels.Where(h => h.PricePerNight > 100 && h.PricePerNight <= 300),
            "luxury" => hotels.Where(h => h.PricePerNight > 300),
            _ => hotels
        };
    }

    private static IEnumerable<dynamic> FilterCarsByBudgetAndSeats(IEnumerable<dynamic> cars, string budgetLevel, int? minSeats)
    {
        var filtered = cars.AsEnumerable();

        // Filter by budget
        filtered = budgetLevel.ToLower() switch
        {
            "budget" => filtered.Where(c => c.Price <= 50),
            "mid-range" or "mid_range" or "midrange" => filtered.Where(c => c.Price > 50 && c.Price <= 150),
            "luxury" => filtered.Where(c => c.Price > 150),
            _ => filtered
        };

        // Filter by seats if specified
        if (minSeats.HasValue)
        {
            filtered = filtered.Where(c => c.Seats >= minSeats.Value);
        }

        return filtered;
    }
}
