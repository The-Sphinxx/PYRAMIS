using System.Text.Json;
using Agentic_Rentify.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Agentic_Rentify.Infragentic.Services;

/// <summary>
/// Two-Stage AI Trip Planner Implementation
/// Stage 1: SmartBrain (Grok) - Orchestrator - Fetches real SQL data
/// Stage 2: FastBrain (GPT-OSS) - Formatter - Generates UI-ready JSON
/// </summary>
public class TwoStageTripPlannerService(
    Kernel kernel,
    IAIBrainService aiBrainService,
    ILogger<TwoStageTripPlannerService> logger) : IAiTripPlannerService
{
    private readonly Kernel _kernel = kernel;
    private readonly IAIBrainService _aiBrainService = aiBrainService;
    private readonly ILogger<TwoStageTripPlannerService> _logger = logger;

    public async Task<string> GenerateTripPlanAsync(
        string destination,
        DateTime startDate,
        DateTime endDate,
        string budgetLevel,
        int travelers,
        List<string> interests)
    {
        var tripDays = (endDate - startDate).Days + 1;
        var interestsText = interests.Any() ? string.Join(", ", interests) : "general sightseeing";

        try
        {
            _logger.LogInformation("🚀 Starting Two-Stage Trip Planning: {Destination}, {Days} days, {Budget}, {Travelers} travelers",
                destination, tripDays, budgetLevel, travelers);

            // ============================================================
            // STAGE 1: SMART ORCHESTRATOR (Grok) - Fetch Real Data
            // ============================================================
            _logger.LogInformation("🧠 Stage 1: SmartBrain (Grok) - Fetching real data...");
            
            var stage1Result = await ExecuteStage1OrchestratorAsync(
                destination, startDate, endDate, budgetLevel, travelers, interestsText, tripDays);

            if (stage1Result == null || !stage1Result.Success)
            {
                _logger.LogWarning("⚠️ Stage 1 failed or returned no data. Reason: {Reason}", 
                    stage1Result?.ErrorMessage ?? "Unknown");
                return GenerateFallbackPlan(destination, startDate, endDate, budgetLevel, tripDays);
            }

            _logger.LogInformation("✅ Stage 1 complete. Hotels: {HotelCount}, Attractions: {AttractionCount}, Vehicles: {VehicleCount}",
                stage1Result.Hotels.Count, stage1Result.Attractions.Count, stage1Result.Vehicles.Count);

            // ============================================================
            // STAGE 2: UI FORMATTER (FastBrain GPT-OSS) - Generate JSON
            // ============================================================
            _logger.LogInformation("⚡ Stage 2: FastBrain (GPT-OSS) - Generating UI-ready JSON...");
            
            var finalJson = await ExecuteStage2FormatterAsync(
                stage1Result, destination, startDate, endDate, budgetLevel, travelers, interestsText, tripDays);

            // Convert snake_case to camelCase for API response
            var convertedJson = ConvertSnakeCaseToCamelCase(finalJson);

            _logger.LogInformation("✅ Two-Stage Trip Planning completed successfully");
            return convertedJson;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Two-Stage Trip Planner failed: {Message}", ex.Message);
            return GenerateFallbackPlan(destination, startDate, endDate, budgetLevel, tripDays);
        }
    }

    /// <summary>
    /// Stage 1: Smart Orchestrator (Grok) - Fetches real data using SQL Tools
    /// </summary>
    private async Task<Stage1Result?> ExecuteStage1OrchestratorAsync(
        string destination,
        DateTime startDate,
        DateTime endDate,
        string budgetLevel,
        int travelers,
        string interests,
        int tripDays)
    {
        var stage1Prompt = $$$"""
You are the SMART ORCHESTRATOR (Grok). Your job is to gather real data for a trip plan.

**USER REQUIREMENTS:**
- Destination: {{{destination}}}
- Dates: {{{startDate:yyyy-MM-dd}}} to {{{endDate:yyyy-MM-dd}}} ({{{tripDays}}} days)
- Budget: {{{budgetLevel}}}
- Travelers: {{{travelers}}}
- Interests: {{{interests}}}

**YOUR TASK:**
Use the DataFetch plugin tools to gather COMPLETE DTO objects (the "Ingredients"):

1. Call DataFetch-fetch_hotels_for_planner
   - Pass: destination="{{{destination}}}", budgetLevel="{{{budgetLevel}}}", topK=3
   
2. Call DataFetch-fetch_attractions_for_planner
   - Pass: destination="{{{destination}}}", interests="{{{interests}}}", topK=10
   
3. Call DataFetch-fetch_vehicles_for_planner
   - Pass: destination="{{{destination}}}", travelers={{{travelers}}}, budgetLevel="{{{budgetLevel}}}", topK=3

**CRITICAL RULES:**
- You MUST invoke all three tools in parallel using Task.WhenAll semantics
- Return the COMPLETE JSON responses from each tool
- Do NOT filter, modify, or summarize the data
- If a tool returns success=false, include the alternative_suggestion in your response
- Ensure latitude/longitude are NEVER 0 for any entity

**OUTPUT FORMAT:**
Return a JSON object containing the raw results from all three tools:

```json
{
  "hotels_result": [...full result from fetch_hotels_for_planner...],
  "attractions_result": [...full result from fetch_attractions_for_planner...],
  "vehicles_result": [...full result from fetch_vehicles_for_planner...]
}
```

Execute the tools NOW and return the aggregated results.
""";

        var executionSettings = new OpenAIPromptExecutionSettings
        {
            Temperature = 0.3, // Low temperature for consistent data fetching
            MaxTokens = 4000,
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };

        var kernelArgs = new KernelArguments(executionSettings) { ["prompt"] = stage1Prompt };
        var stage1Function = _kernel.CreateFunctionFromPrompt(stage1Prompt);

        var result = await _aiBrainService.InvokeWithFallbackAsync(
            AIBrain.SmartBrain, // Grok for intelligent orchestration
            stage1Function,
            kernelArgs,
            onFallback: (failedBrain, ex) =>
            {
                _logger.LogWarning("⚠️ Stage 1 fallback: {Brain} → DefaultBrain. Reason: {Message}", 
                    failedBrain, ex.Message);
            }
        );

        var rawJson = result.ToString();
        _logger.LogDebug("Stage 1 Raw Response: {Response}", rawJson.Substring(0, Math.Min(500, rawJson.Length)));

        return ParseStage1Result(rawJson);
    }

    /// <summary>
    /// Stage 2: UI Formatter (FastBrain GPT-OSS) - Generates UI-ready JSON
    /// </summary>
    private async Task<string> ExecuteStage2FormatterAsync(
        Stage1Result contextPackage,
        string destination,
        DateTime startDate,
        DateTime endDate,
        string budgetLevel,
        int travelers,
        string interests,
        int tripDays)
    {
        var stage2Prompt = $$$"""
You are the UI FORMATTER (FastBrain GPT-OSS). You receive COMPLETE data objects from Stage 1.

**CONTEXT PACKAGE (The Ingredients):**
Hotels: {{{JsonSerializer.Serialize(contextPackage.Hotels)}}}
Attractions: {{{JsonSerializer.Serialize(contextPackage.Attractions)}}}
Vehicles: {{{JsonSerializer.Serialize(contextPackage.Vehicles)}}}

**USER REQUIREMENTS:**
- Destination: {{{destination}}}
- Dates: {{{startDate:yyyy-MM-dd}}} to {{{endDate:yyyy-MM-dd}}} ({{{tripDays}}} days)
- Budget: {{{budgetLevel}}}
- Travelers: {{{travelers}}}
- Interests: {{{interests}}}

**YOUR TASK:**
Transform the Context Package into a PERFECT JSON that matches the Vue.js UI components.

**CRITICAL UI REQUIREMENTS:**
1. HotelCard.vue expects: { hotel: { name, image, price, location, rating, reviews, amenities[] } }
2. AttractionCard.vue expects: { id, image, title, price, location, rating, reviews }
3. CarCard.vue expects: { car: { name, overview, images[], price, reviews: { overallRating, totalReviews } } }

**BUDGET CALCULATIONS:**
- Accommodation: SUM(selected hotels × nights)
- Activities: SUM(attraction prices × estimated visits)
- Transportation: vehicle price × days
- Grand Total: accommodation + activities + transportation
- Per Person: grand_total / {{{travelers}}}

**ITINERARY RULES:**
- Create {{{tripDays}}} days
- Distribute 2-4 activities per day from the attractions list
- Use ONLY attractions from the Context Package (with their exact IDs, names, prices, coordinates)
- Ensure each activity has: name, time, description, duration_hours, price, latitude, longitude, activity_type

**OUTPUT FORMAT (STRICT JSON):**
Return a valid JSON object with this exact structure:
- trip_overview: destination, start_date, end_date, duration_days, travelers, budget_level
- estimated_costs: accommodation_total, activities_total, transportation_total, grand_total, currency, per_person
- lodging_recommendations: array of hotels with hotel_id, name, city, price_per_night, total_nights, total_cost, rating, amenities[], latitude, longitude, images[], reason
- vehicle_recommendation: vehicle_id, name, type, seats, price_per_day, total_days, total_cost, images[]
- itinerary: array of days with day, date, title, activities[], meals (object: { breakfast, lunch, dinner }), accommodation (object: { hotel_id, name })
- travel_tips: array of helpful tips

**VALIDATION RULES:**
- latitude/longitude MUST NEVER be 0 (use Context Package values)
- images[] arrays MUST NOT be empty if Context Package has images
- All prices MUST be numeric (extract from Context Package)
- Total costs MUST be mathematically accurate

Generate the JSON NOW.
""";

        var executionSettings = new OpenAIPromptExecutionSettings
        {
            Temperature = 0.7, // Moderate for creative itinerary building
            MaxTokens = 6000,
            ToolCallBehavior = ToolCallBehavior.EnableKernelFunctions // No auto-invoke, pure formatting
        };

#pragma warning disable SKEXP0010
        executionSettings.ResponseFormat = "json_object"; // Force JSON output
#pragma warning restore SKEXP0010

        var kernelArgs = new KernelArguments(executionSettings) { ["prompt"] = stage2Prompt };
        var stage2Function = _kernel.CreateFunctionFromPrompt(stage2Prompt);

        var result = await _aiBrainService.InvokeWithFallbackAsync(
            AIBrain.FastBrain, // GPT-OSS for fast JSON generation
            stage2Function,
            kernelArgs,
            onFallback: (failedBrain, ex) =>
            {
                _logger.LogWarning("⚠️ Stage 2 fallback: {Brain} → DefaultBrain. Reason: {Message}", 
                    failedBrain, ex.Message);
            }
        );

        var finalJson = result.ToString();

        // Validate JSON structure
        ValidateUICompatibility(finalJson);

        // Normalize differences that break DTO deserialization (e.g., meals array → object)
        finalJson = NormalizeFormatterOutput(finalJson);

        return finalJson;
    }

    // Helper Classes
    private class Stage1Result
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public List<dynamic> Hotels { get; set; } = new();
        public List<dynamic> Attractions { get; set; } = new();
        public List<dynamic> Vehicles { get; set; } = new();
    }

    private Stage1Result? ParseStage1Result(string rawJson)
    {
        try
        {
            var doc = JsonDocument.Parse(rawJson);
            var root = doc.RootElement;

            var result = new Stage1Result { Success = true };

            // Extract hotels - handle null result object
            if (root.TryGetProperty("hotels_result", out var hotelsResult) && 
                hotelsResult.ValueKind != JsonValueKind.Null)
            {
                if (hotelsResult.TryGetProperty("hotels", out var hotelsArray))
                {
                    result.Hotels = JsonSerializer.Deserialize<List<dynamic>>(hotelsArray.GetRawText()) ?? new();
                }
            }

            // Extract attractions - handle null result object
            if (root.TryGetProperty("attractions_result", out var attractionsResult) && 
                attractionsResult.ValueKind != JsonValueKind.Null)
            {
                if (attractionsResult.TryGetProperty("attractions", out var attractionsArray))
                {
                    result.Attractions = JsonSerializer.Deserialize<List<dynamic>>(attractionsArray.GetRawText()) ?? new();
                }
            }

            // Extract vehicles - handle null result object
            if (root.TryGetProperty("vehicles_result", out var vehiclesResult) && 
                vehiclesResult.ValueKind != JsonValueKind.Null)
            {
                if (vehiclesResult.TryGetProperty("vehicles", out var vehiclesArray))
                {
                    result.Vehicles = JsonSerializer.Deserialize<List<dynamic>>(vehiclesArray.GetRawText()) ?? new();
                }
            }

            result.Success = result.Hotels.Any() || result.Attractions.Any() || result.Vehicles.Any();
            
            if (!result.Success)
            {
                result.ErrorMessage = "No data returned from any tool";
                _logger.LogWarning("⚠️ Stage 1 returned no data. Hotels: {H}, Attractions: {A}, Vehicles: {V}", 
                    result.Hotels.Count, result.Attractions.Count, result.Vehicles.Count);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse Stage 1 result");
            return new Stage1Result 
            { 
                Success = false, 
                ErrorMessage = $"Parse error: {ex.Message}" 
            };
        }
    }

    private void ValidateUICompatibility(string json)
    {
        try
        {
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Check required sections
            var requiredSections = new[] 
            { 
                "trip_overview", "estimated_costs", "lodging_recommendations", 
                "vehicle_recommendation", "itinerary" 
            };

            foreach (var section in requiredSections)
            {
                if (!root.TryGetProperty(section, out _))
                {
                    _logger.LogWarning("⚠️ Missing required section: {Section}", section);
                }
            }

            // Validate coordinates
            if (root.TryGetProperty("itinerary", out var itinerary))
            {
                foreach (var day in itinerary.EnumerateArray())
                {
                    if (day.TryGetProperty("activities", out var activities))
                    {
                        foreach (var activity in activities.EnumerateArray())
                        {
                            if (activity.TryGetProperty("latitude", out var lat) && lat.GetDouble() == 0)
                            {
                                _logger.LogWarning("⚠️ Activity has invalid latitude (0)");
                            }
                        }
                    }
                }
            }

            _logger.LogInformation("✅ UI compatibility validation passed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ UI validation failed: {Message}", ex.Message);
        }
    }

    private string GenerateFallbackPlan(
        string destination, 
        DateTime startDate, 
        DateTime endDate, 
        string budgetLevel,
        int tripDays)
    {
        _logger.LogWarning("⚠️ Generating fallback plan for {Destination}", destination);

        return JsonSerializer.Serialize(new
        {
            trip_overview = new
            {
                destination,
                start_date = startDate.ToString("yyyy-MM-dd"),
                end_date = endDate.ToString("yyyy-MM-dd"),
                duration_days = tripDays,
                travelers = 2,
                budget_level = budgetLevel
            },
            estimated_costs = new
            {
                accommodation_total = 0,
                activities_total = 0,
                transportation_total = 0,
                grand_total = 0,
                currency = "USD",
                per_person = 0
            },
            lodging_recommendations = Array.Empty<object>(),
            vehicle_recommendation = new { },
            itinerary = Array.Empty<object>(),
            travel_tips = new[] 
            { 
                "Unable to generate trip plan. Please try again with different criteria.",
                $"Consider searching for hotels and attractions in {destination} manually."
            },
            error = true,
            error_message = "AI trip planner is temporarily unavailable. Please try again later."
        });
    }

    /// <summary>
    /// Convert snake_case keys to camelCase for API response compatibility
    /// Performs a deep traversal of the JSON and renames object property keys.
    /// </summary>
    private string ConvertSnakeCaseToCamelCase(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var converted = ConvertElementKeysToCamelCase(doc.RootElement);
            return JsonSerializer.Serialize(converted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to convert snake_case to camelCase: {Message}", ex.Message);
            return json; // Return original if conversion fails
        }
    }

    /// <summary>
    /// Normalize Stage 2 formatter output to match strict DTOs.
    /// - Convert meals arrays [breakfast, lunch, dinner] into an object { breakfast, lunch, dinner }
    /// Works for both snake_case (meals) and camelCase (meals).
    /// </summary>
    private string NormalizeFormatterOutput(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            object? NormalizeElement(JsonElement el)
            {
                if (el.ValueKind == JsonValueKind.Object)
                {
                    var dict = new Dictionary<string, object?>();
                    foreach (var prop in el.EnumerateObject())
                    {
                        var name = prop.Name;
                        // Detect itinerary day objects and fix meals if array
                        if (string.Equals(name, "meals", StringComparison.OrdinalIgnoreCase) && prop.Value.ValueKind == JsonValueKind.Array)
                        {
                            var items = prop.Value.EnumerateArray().Select(i => i.ValueKind == JsonValueKind.String ? i.GetString() ?? string.Empty : i.ToString()).ToList();
                            var breakfast = items.ElementAtOrDefault(0) ?? string.Empty;
                            var lunch = items.ElementAtOrDefault(1) ?? string.Empty;
                            var dinner = items.ElementAtOrDefault(2) ?? string.Empty;
                            dict[name] = new Dictionary<string, object?>
                            {
                                { "breakfast", breakfast },
                                { "lunch", lunch },
                                { "dinner", dinner }
                            };
                            continue;
                        }

                        dict[name] = NormalizeElement(prop.Value);
                    }
                    return dict;
                }
                if (el.ValueKind == JsonValueKind.Array)
                {
                    var list = new List<object?>();
                    foreach (var item in el.EnumerateArray())
                    {
                        list.Add(NormalizeElement(item));
                    }
                    return list;
                }
                // primitives
                switch (el.ValueKind)
                {
                    case JsonValueKind.String: return el.GetString();
                    case JsonValueKind.Number:
                        if (el.TryGetInt64(out var l)) return l;
                        if (el.TryGetDouble(out var d)) return d;
                        return el.GetDecimal();
                    case JsonValueKind.True: return true;
                    case JsonValueKind.False: return false;
                    case JsonValueKind.Null: return null;
                    default: return null;
                }
            }

            var normalized = NormalizeElement(root);
            return JsonSerializer.Serialize(normalized);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to normalize formatter output: {Message}", ex.Message);
            return json;
        }
    }

    private object? ConvertElementKeysToCamelCase(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var dict = new Dictionary<string, object?>();
                foreach (var prop in element.EnumerateObject())
                {
                    var camelName = ToCamelCase(prop.Name);
                    dict[camelName] = ConvertElementKeysToCamelCase(prop.Value);
                }
                return dict;

            case JsonValueKind.Array:
                var list = new List<object?>();
                foreach (var item in element.EnumerateArray())
                {
                    list.Add(ConvertElementKeysToCamelCase(item));
                }
                return list;

            case JsonValueKind.String:
                return element.GetString();
            case JsonValueKind.Number:
                if (element.TryGetInt64(out var l)) return l;
                if (element.TryGetDouble(out var d)) return d;
                return element.GetDecimal();
            case JsonValueKind.True:
                return true;
            case JsonValueKind.False:
                return false;
            case JsonValueKind.Null:
                return null;
            default:
                return null;
        }
    }

    private static string ToCamelCase(string snake)
    {
        if (string.IsNullOrEmpty(snake)) return snake;

        // If already contains uppercase letters, assume not snake_case
        var hasUpper = snake.Any(char.IsUpper);
        var containsUnderscore = snake.Contains('_');
        if (!containsUnderscore)
        {
            // Ensure first letter lowercased for consistency
            return char.ToLowerInvariant(snake[0]) + (snake.Length > 1 ? snake.Substring(1) : string.Empty);
        }

        var parts = snake.Split('_', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return snake;
        var sb = new System.Text.StringBuilder(parts[0].ToLowerInvariant());
        for (int i = 1; i < parts.Length; i++)
        {
            var p = parts[i];
            if (p.Length == 0) continue;
            sb.Append(char.ToUpperInvariant(p[0]));
            if (p.Length > 1) sb.Append(p.Substring(1));
        }
        return sb.ToString();
    }
}