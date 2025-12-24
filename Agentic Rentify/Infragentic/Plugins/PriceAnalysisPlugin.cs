using System.ComponentModel;
using System.Text.Json;
using Agentic_Rentify.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

namespace Agentic_Rentify.Infragentic.Plugins;

/// <summary>
/// Provides real-time price analysis from the database for budget planning
/// </summary>
public class PriceAnalysisPlugin(IServiceScopeFactory serviceScopeFactory)
{
    [KernelFunction("get_price_ranges")]
    [Description("Gets REAL price ranges (min/max) for all entity types (hotels, cars, attractions, trips) in a specific destination from the database. Use this to understand actual budget constraints.")]
    public async Task<string> GetPriceRangesAsync(
        [Description("Destination city (e.g., 'Cairo', 'Luxor')")] string destination)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var sqlPlugin = scope.ServiceProvider.GetRequiredService<SqlQueryPlugin>();

        try
        {
            // Query real database for price ranges
            var hotelPrices = await sqlPlugin.ExecuteScalarQueryAsync(
                $@"SELECT 
                    MIN(CAST(REPLACE(REPLACE(PricePerNight, '$', ''), ',', '') AS DECIMAL(10,2))) as MinPrice,
                    MAX(CAST(REPLACE(REPLACE(PricePerNight, '$', ''), ',', '') AS DECIMAL(10,2))) as MaxPrice,
                    AVG(CAST(REPLACE(REPLACE(PricePerNight, '$', ''), ',', '') AS DECIMAL(10,2))) as AvgPrice
                FROM Hotels 
                WHERE City = '{destination.Trim()}' AND PricePerNight IS NOT NULL");

            var carPrices = await sqlPlugin.ExecuteScalarQueryAsync(
                $@"SELECT 
                    MIN(CAST(REPLACE(REPLACE(PricePerDay, '$', ''), ',', '') AS DECIMAL(10,2))) as MinPrice,
                    MAX(CAST(REPLACE(REPLACE(PricePerDay, '$', ''), ',', '') AS DECIMAL(10,2))) as MaxPrice,
                    AVG(CAST(REPLACE(REPLACE(PricePerDay, '$', ''), ',', '') AS DECIMAL(10,2))) as AvgPrice
                FROM Cars 
                WHERE City = '{destination.Trim()}' AND PricePerDay IS NOT NULL");

            var attractionPrices = await sqlPlugin.ExecuteScalarQueryAsync(
                $@"SELECT 
                    MIN(CAST(Price AS DECIMAL(10,2))) as MinPrice,
                    MAX(CAST(Price AS DECIMAL(10,2))) as MaxPrice,
                    AVG(CAST(Price AS DECIMAL(10,2))) as AvgPrice
                FROM Attractions 
                WHERE Location = '{destination.Trim()}' AND Price IS NOT NULL");

            return JsonSerializer.Serialize(new
            {
                success = true,
                destination,
                priceRanges = new
                {
                    hotels = ParsePriceResult(hotelPrices, "per night"),
                    cars = ParsePriceResult(carPrices, "per day"),
                    attractions = ParsePriceResult(attractionPrices, "per visit"),
                },
                budgetGuidelines = new
                {
                    budget = "Total trip < $800 per person",
                    midRange = "Total trip $800-$2000 per person",
                    luxury = "Total trip > $2000 per person"
                }
            });
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = ex.Message,
                fallback = new
                {
                    hotels = new { min = 50, max = 300, avg = 150, unit = "per night" },
                    cars = new { min = 30, max = 200, avg = 80, unit = "per day" },
                    attractions = new { min = 0, max = 250, avg = 50, unit = "per visit" }
                }
            });
        }
    }

    [KernelFunction("check_availability")]
    [Description("Checks real-time availability for hotels and cars in a destination for specific dates. Returns counts and status.")]
    public async Task<string> CheckAvailabilityAsync(
        [Description("Destination city")] string destination,
        [Description("Start date (YYYY-MM-DD)")] string startDate,
        [Description("End date (YYYY-MM-DD)")] string endDate)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var sqlPlugin = scope.ServiceProvider.GetRequiredService<SqlQueryPlugin>();

        try
        {
            // Check hotel availability (not booked for those dates)
            var hotelAvailability = await sqlPlugin.ExecuteScalarQueryAsync(
                $@"SELECT COUNT(*) as AvailableCount
                FROM Hotels h
                WHERE h.City = '{destination.Trim()}'
                AND h.Id NOT IN (
                    SELECT HotelId FROM Bookings 
                    WHERE HotelId IS NOT NULL 
                    AND Status IN ('Confirmed', 'Pending')
                    AND ((StartDate BETWEEN '{startDate}' AND '{endDate}')
                         OR (EndDate BETWEEN '{startDate}' AND '{endDate}'))
                )");

            // Check car availability
            var carAvailability = await sqlPlugin.ExecuteScalarQueryAsync(
                $@"SELECT 
                    COUNT(*) as AvailableCount,
                    SUM(CASE WHEN Status = 'Available' THEN 1 ELSE 0 END) as InStockCount
                FROM Cars c
                WHERE c.City = '{destination.Trim()}'");

            return JsonSerializer.Serialize(new
            {
                success = true,
                destination,
                dateRange = new { startDate, endDate },
                availability = new
                {
                    hotels = ParseAvailabilityResult(hotelAvailability),
                    cars = ParseAvailabilityResult(carAvailability)
                }
            });
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = ex.Message
            });
        }
    }

    private static object ParsePriceResult(string jsonResult, string unit)
    {
        try
        {
            var data = JsonSerializer.Deserialize<Dictionary<string, decimal>>(jsonResult);
            if (data == null) return new { min = 0, max = 0, avg = 0, unit };

            return new
            {
                min = data.GetValueOrDefault("MinPrice", 0),
                max = data.GetValueOrDefault("MaxPrice", 0),
                avg = Math.Round(data.GetValueOrDefault("AvgPrice", 0), 2),
                unit
            };
        }
        catch
        {
            return new { min = 0, max = 0, avg = 0, unit };
        }
    }

    private static object ParseAvailabilityResult(string jsonResult)
    {
        try
        {
            var data = JsonSerializer.Deserialize<Dictionary<string, int>>(jsonResult);
            return new
            {
                available = data?.GetValueOrDefault("AvailableCount", 0) ?? 0,
                inStock = data?.GetValueOrDefault("InStockCount", 0) ?? 0
            };
        }
        catch
        {
            return new { available = 0, inStock = 0 };
        }
    }
}
