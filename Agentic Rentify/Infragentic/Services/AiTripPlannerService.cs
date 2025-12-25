using Agentic_Rentify.Application.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Text.Json;

namespace Agentic_Rentify.Infragentic.Services;

public class AiTripPlannerService(
    Kernel kernel,
    IAIBrainService aiBrainService) : IAiTripPlannerService
{
    private readonly Kernel _kernel = kernel;
    private readonly IAIBrainService _aiBrainService = aiBrainService;

    public async Task<string> GenerateTripPlanAsync(
        string destination,
        DateTime startDate,
        DateTime endDate,
        string budgetLevel,
        int travelers,
        List<string> interests)
    {
        var tripDays = (endDate - startDate).Days + 1;

        // Build the sophisticated prompt for trip planning
        var plannerPrompt = BuildTripPlannerPrompt(destination, startDate, endDate, budgetLevel, travelers, interests, tripDays);

        // Execution settings optimized for FastBrain JSON generation
        var executionSettings = new OpenAIPromptExecutionSettings
        {
            Temperature = 0.7,
            MaxTokens = 4000,
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
        };

        // Force JSON output (using experimental API - suppress warning)
#pragma warning disable SKEXP0010
        executionSettings.ResponseFormat = "json_object";
#pragma warning restore SKEXP0010

        try
        {
            // Use FastBrain for optimized trip planning with fallback to DefaultBrain
            var kernelArgs = new KernelArguments(executionSettings) { ["prompt"] = plannerPrompt };
            
            var result = await _aiBrainService.InvokeWithFallbackAsync(
                AIBrain.FastBrain,
                _kernel.CreateFunctionFromPrompt(plannerPrompt),
                kernelArgs,
                onFallback: (failedBrain, ex) =>
                {
                    // Log fallback event
                    Console.WriteLine($"[FALLBACK] Trip planner switched from {failedBrain} to DefaultBrain: {ex.Message}");
                }
            );

            return result.ToString();
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new
            {
                error = "Failed to generate trip plan",
                message = ex.Message,
                fallback = GenerateFallbackPlan(destination, startDate, endDate, budgetLevel)
            });
        }
    }

    private static string BuildTripPlannerPrompt(
        string destination,
        DateTime startDate,
        DateTime endDate,
        string budgetLevel,
        int travelers,
        List<string> interests,
        int tripDays)
    {
        var interestsText = interests.Any() ? string.Join(", ", interests) : "general sightseeing";

        return $$$"""
You are an expert AI Travel Planner specializing in creating personalized, realistic itineraries for Egypt.

**USER REQUIREMENTS:**
- Destination: {{{destination}}}
- Travel Dates: {{{startDate:yyyy-MM-dd}}} to {{{endDate:yyyy-MM-dd}}} ({{{tripDays}}} days)
- Budget Level: {{{budgetLevel}}}
- Number of Travelers: {{{travelers}}}
- Interests: {{{interestsText}}}

**YOUR TASK:**
Create a comprehensive, day-by-day trip plan using ONLY real entities from the database. You MUST use the available tools to search for actual hotels, attractions, and vehicles.

**REQUIRED TOOLS TO USE:**
1. TravelDatabase-search_hotels_by_destination: Find real hotels in {{{destination}}} matching the {{{budgetLevel}}} budget
2. TravelDatabase-search_attractions_by_destination: Find attractions matching interests: {{{interestsText}}}
3. TravelDatabase-search_vehicles_by_criteria: Find rental vehicles for {{{travelers}}} travelers at {{{budgetLevel}}} budget

**CRITICAL CONSTRAINTS:**
- Use ONLY entities returned from the TravelDatabase plugin functions
- Each hotel, attraction, and vehicle must have real IDs, names, and coordinates
- Calculate costs based on actual pricePerNight, prices, and number of days
- Include latitude and longitude for ALL locations to enable map markers
- Ensure activities match the user's stated interests
- Distribute activities reasonably across {{{tripDays}}} days

**OUTPUT FORMAT (JSON):**
Return a valid JSON object with the following structure:

```json
{
  "trip_overview": {
    "destination": "{{{destination}}}",
    "start_date": "{{{startDate:yyyy-MM-dd}}}",
    "end_date": "{{{endDate:yyyy-MM-dd}}}",
    "duration_days": {{{tripDays}}},
    "travelers": {{{travelers}}},
    "budget_level": "{{{budgetLevel}}}"
  },
  "estimated_costs": {
    "accommodation_total": 0,
    "activities_total": 0,
    "transportation_total": 0,
    "grand_total": 0,
    "currency": "USD",
    "per_person": 0
  },
  "lodging_recommendations": [
    {
      "hotel_id": 0,
      "name": "",
      "city": "",
      "price_per_night": 0,
      "total_nights": {{{tripDays - 1}}},
      "total_cost": 0,
      "rating": 0,
      "amenities": [],
      "latitude": 0,
      "longitude": 0,
      "images": [],
      "reason": "Why this hotel fits the traveler's needs"
    }
  ],
  "vehicle_recommendation": {
    "vehicle_id": 0,
    "name": "",
    "type": "",
    "seats": 0,
    "price_per_day": 0,
    "total_days": {{{tripDays}}},
    "total_cost": 0,
    "features": [],
    "images": []
  },
  "itinerary": [
    {
      "day": 1,
      "date": "{{{startDate:yyyy-MM-dd}}}",
      "title": "Day 1 Title",
      "activities": [
        {
          "time": "09:00 AM",
          "activity_type": "attraction",
          "attraction_id": 0,
          "name": "",
          "description": "",
          "duration_hours": 2,
          "price": 0,
          "latitude": 0,
          "longitude": 0,
          "images": []
        }
      ],
      "meals": {
        "breakfast": "Hotel breakfast",
        "lunch": "Local restaurant recommendation",
        "dinner": "Dining suggestion"
      },
      "accommodation": {
        "hotel_id": 0,
        "name": ""
      }
    }
  ],
  "travel_tips": [
    "Tip 1 specific to {{{destination}}}",
    "Tip 2 based on {{{budgetLevel}}} budget",
    "Tip 3 relevant to {{{interestsText}}}"
  ],
  "booking_summary": {
    "hotels_to_book": [],
    "attractions_to_book": [],
    "vehicle_to_book": null
  }
}
```

**IMPORTANT REMINDERS:**
- Start by calling the TravelDatabase plugin functions to get REAL data
- Never invent or hallucinate hotel names, prices, or coordinates
- Ensure JSON is valid and parseable
- Each attraction must include coordinates for map plotting
- Calculate all costs accurately based on returned prices
- Match activities to the user's interests: {{{interestsText}}}
- Make the itinerary realistic and executable

Generate the trip plan now:
""";
    }

    private static string GenerateFallbackPlan(string destination, DateTime startDate, DateTime endDate, string budgetLevel)
    {
        var tripDays = (endDate - startDate).Days + 1;
        return JsonSerializer.Serialize(new
        {
            trip_overview = new
            {
                destination,
                start_date = startDate.ToString("yyyy-MM-dd"),
                end_date = endDate.ToString("yyyy-MM-dd"),
                duration_days = tripDays,
                budget_level = budgetLevel,
                note = "This is a fallback plan. Please try again or contact support."
            },
            estimated_costs = new
            {
                message = "Unable to calculate costs at this time"
            },
            lodging_recommendations = Array.Empty<object>(),
            itinerary = Array.Empty<object>(),
            error = "Unable to generate detailed plan. Please verify destination and try again."
        });
    }
}
