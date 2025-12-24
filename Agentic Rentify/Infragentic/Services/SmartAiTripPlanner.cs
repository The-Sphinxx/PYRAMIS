using System.Text.Json;
using System.Text.RegularExpressions;
using Agentic_Rentify.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Agentic_Rentify.Infragentic.Services;

public class SmartAiTripPlannerService(
    Kernel kernel,
    IAIBrainService aiBrainService,
    ILogger<SmartAiTripPlannerService> logger) : IAiTripPlannerService
{
    public async Task<string> GenerateTripPlanAsync(
        string destination,
        DateTime startDate,
        DateTime endDate,
        string preferences, 
        int travelers,
        List<string> interests)
    {
        var tripDays = (endDate - startDate).Days + 1;
        var interestsText = string.Join(", ", interests);

       var masterPrompt = $$$"""
        <system_role>
        You are the "Deterministic SQL Travel Agent". 
        DIALECT: T-SQL (SQL Server) ONLY.
        FORBIDDEN: DO NOT use 'LIMIT'. DO NOT call 'get_table_schema'. 
        MANDATORY: Use 'SELECT TOP 3' for all queries.
        </system_role>
        
        <system_instruction>
        You are the "Master Egypt Architect". 
        RULE 1: Use T-SQL (SELECT TOP 3).
        RULE 2: If Latitude/Longitude/Price is 0 in DB, use these defaults: Lat: 30.0444, Lon: 31.2357, Price: 50.
        RULE 3: Return ONLY ONE JSON.
        RULE 4: Use the following database schema for queries.
        Rule 5: Never leave tips empty or with placeholders.
        Rule 6: For ITINERARY, Never leave any day with Date Placeholder use start date at day 1 and increment for subsequent days.
        Rule 7: For ITINERARY, Never Leave any activity without a time or title.
        Rule 8: For ITINERARY, Use images from DB.
        Rule 9: For ITINERARY, Never leave any activity empty.
        Rule 10: For ITINERARY, FILL ALL MEALS With actual meal suggestions NEVER LEAVE ANY MEAL EMPTY.
        </system_instruction>

        <workflow_sequence>
        STEP 1: Run SQL for Hotels in {{{destination}}}.
        STEP 2: Run SQL for Cars in {{{destination}}}.
        STEP 3: Run SQL for Attractions in {{{destination}}}.
        STEP 4: Run SQL for Trips in {{{destination}}}.
        STEP 6: Synthesize History and Summary for {{{destination}}}.
        STEP 7: Build ONE FINAL JSON block and STOP.
        </workflow_sequence>

        <database_schema_reference>
        - Hotels: Id, Name, City, Rating, PricePerNight, Images, Amenities, Latitude, Longitude.
        - Attractions: Id, Name, City, Price, Rating, Images, Latitude, Longitude.
        - Cars: Id, Name, Brand, Price, ReviewSummary, Images.
        - Trips: Id, Title, City, Price, Rating, Duration.
        </database_schema_reference>

        <user_input>
        Target: {{{destination}}}, Days: {{{tripDays}}}, Start: {{{startDate:yyyy-MM-dd}}}
        </user_input>

        <output_constraint>
        Return ONLY the raw JSON. If SQL fails, use your internal knowledge for Egypt to fill the fields instead of returning zeros.
        </output_constraint>
        <json_schema>
        {
        "title": "", "summary": "", "historical_background": "",
        "estimated_costs": { "total_per_day": 0, "grand_total": 0, "breakdown": { "stay": 0, "trips": 0, "car": 0, "activities": 0 } },
        "lodging_recommendations": [ { "hotel": { "name": "", "image": "", "price": 0, "location": "", "rating": 0, "reviews": "100 reviews", "amenities": [] } } ],
        "car_recommendations": [ { "car": { "name": "", "overview": "", "price": 0, "images": [], "reviews": { "overallRating": 0, "totalReviews": 0 } } } ],
        "trip_recommendations": [],
        "attraction_recommendations": [ { "attraction": { "id": 0, "image": "", "title": "", "price": 0, "location": "", "rating": 0, "reviews": 0 } } ],
        "itinerary": [ { "day": 1, "date": "YYYY-MM-DD", "activities": [ { "time": "09:00 AM", "name": "", "duration": "2h", "image": "", "price": 0 } ], "meals": { "breakfast": "", 	"lunch": 	"", "dinner": "" } } ],
        "travel_tips": ["tip1", "tip2"]
        }
        </json_schema>
        """;

        #pragma warning disable SKEXP0001 
        var settings = new OpenAIPromptExecutionSettings { 
            ServiceId = "SmartBrain", 
            Temperature = 0.7, 
            MaxTokens = 4000, 
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions 
        };

        var promptConfig = new PromptTemplateConfig(masterPrompt) { 
            Name = "ExecuteSmartPlan", 
            ExecutionSettings = { { "default", settings } } 
        };

        var plannerFunction = kernel.CreateFunctionFromPrompt(promptConfig);
        
        // تسجيل الوظيفة في Plugin وهمي لضمان ظهور الاسم في الفلتر والـ DB
        var plugin = kernel.ImportPluginFromFunctions("AiPlannerPlugin", [plannerFunction]);

        try {
            var result = await aiBrainService.InvokeWithFallbackAsync(
                AIBrain.SmartBrain, 
                plugin["ExecuteSmartPlan"], 
                new KernelArguments(),
                onFallback: (brain, ex) => logger.LogWarning("⚠️ Fallback to DefaultBrain")
            );
            return result.ToString();
        }
        catch (Exception ex) {
            logger.LogError(ex, "❌ Planning Service Failed");
            return "{}";
        }
        #pragma warning restore SKEXP0001
    }
}