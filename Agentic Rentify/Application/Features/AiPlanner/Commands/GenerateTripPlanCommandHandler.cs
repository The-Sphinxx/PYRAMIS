using System.Text.Json;
using System.Text.RegularExpressions;
using Agentic_Rentify.Application.Features.AiPlanner.DTOs;
using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Application.Features.Hotels.DTOs;
using Agentic_Rentify.Application.Features.Cars.DTOs;
using Agentic_Rentify.Application.Features.Attractions.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Agentic_Rentify.Application.Features.AiPlanner.Commands.GenerateTripPlan;

public class GenerateTripPlanCommandHandler(IAiTripPlannerService aiPlannerService, ILogger<GenerateTripPlanCommandHandler> logger) 
    : IRequestHandler<GenerateTripPlanCommand, TripPlanResponse>
{
    public async Task<TripPlanResponse> Handle(GenerateTripPlanCommand request, CancellationToken cancellationToken)
    {
        var criteria = request.Criteria;
        try {
            // 1. استدعاء الـ AI
            var preferencesText = $"With: {criteria.WithWhom}, Style: {string.Join(",", criteria.TravelStyle)}, Pace: {criteria.TravelPace}, Accomm: {criteria.Accommodation}";
            var aiResultJson = await aiPlannerService.GenerateTripPlanAsync(criteria.HeadingTo, criteria.StartDate, criteria.EndDate, preferencesText, criteria.Travelers, criteria.TravelStyle);

            // 2. استخراج الـ JSON (تنظيف الرد من أي نصوص زائدة)
            var jsonMatch = Regex.Match(aiResultJson, @"\{(?:[^{}]|(?<o>\{)|(?<-o>\}))+(?(o)(?!))\}", RegexOptions.Singleline);
            if (!jsonMatch.Success) throw new Exception("No valid JSON structure found in AI response.");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
            var raw = JsonSerializer.Deserialize<AiRawTripPlanResult>(jsonMatch.Value, options) ?? throw new Exception("Mapping to Raw DTO failed.");

            // 3. بناء الرد النهائي وتلقيم البيانات الأساسية
            var response = new TripPlanResponse {
            Title = raw.Title ?? $"Trip to {criteria.HeadingTo}",
            Summary = raw.Summary,
            HistoricalBackground = raw.HistoricalBackground,
            DurationDays = (criteria.EndDate - criteria.StartDate).Days + 1,
            // نأخذ التواريخ والمسافرين من الـ Criteria مباشرة (أضمن)
            TripOverview = new TripOverviewData {
                StartDate = criteria.StartDate.ToString("yyyy-MM-dd"),
                EndDate = criteria.EndDate.ToString("yyyy-MM-dd"),
                Travelers = criteria.Travelers
            },
            TravelTips = raw.TravelTips,
            Itinerary = raw.Itinerary,
            Success = true
        };

            // 4. الـ Mapping التلقائي للتوصيات (تحويل الأرقام لنصوص منسقة للـ UI)
            MapEntities(raw, response);

            // 5. ميكانيكا الحساب التلقائي (Fallback) - إذا أعاد الـ AI أصفاراً، نحن نحسبها هنا
            CalculateBudget(response, raw);

            return response;
        }
        catch (Exception ex) {
            logger.LogError(ex, "❌ AI Planner Handler Error");
            return new TripPlanResponse { Success = false, ErrorMessage = ex.Message };
        }
    }

    private void MapEntities(AiRawTripPlanResult raw, TripPlanResponse res)
    {
        // 1. الفنادق
    res.LodgingRecommendations = raw.LodgingRecommendations.Select(r => new HotelCardWrapper {
        Hotel = new HotelResponseDTO {
            Name = r.Hotel.Name,
            PricePerNight = $"{r.Hotel.Price} $",
            Rating = r.Hotel.Rating,
            City = r.Hotel.Location,
            Images = new List<string> { r.Hotel.Image },
            Amenities = r.Hotel.Amenities,
            // تصحيح الإحداثيات: لو الـ AI أو الـ DB أعطى 0، نضع إحداثيات مركز القاهرة كـ Fallback
            Latitude = r.Hotel.Latitude != 0 ? r.Hotel.Latitude : 30.0444,
            Longitude = r.Hotel.Longitude != 0 ? r.Hotel.Longitude : 31.2357,
            Status = "Active"
        }
    }).ToList();

    // 2. المعالم (Attractions)
    res.AttractionRecommendations = raw.AttractionRecommendations.Select(r => new AttractionCardWrapper {
        Attraction = new AttractionResponseDTO {
            Id = r.Attraction.Id,
            Name = r.Attraction.Title,
            Price = $"{r.Attraction.Price} EGP",
            Rating = r.Attraction.Rating,
            Images = new List<string> { r.Attraction.Image },
            City = r.Attraction.Location,
            Latitude = r.Attraction.Latitude != 0 ? r.Attraction.Latitude : 30.0444,
            Longitude = r.Attraction.Longitude != 0 ? r.Attraction.Longitude : 31.2357,
            Status = "Active"
        }
    }).ToList();

    // 3. السيارات
    res.CarRecommendations = raw.CarRecommendations.Select(r => new CarCardWrapper {
        Car = new CarResponseDTO {
            Name = r.Car.Name,
            Price = $"{r.Car.Price} $",
            Overview = r.Car.Overview,
            Images = r.Car.Images,
            Status = "Available"
        }
    }).ToList();
    }

    private void CalculateBudget(TripPlanResponse res, AiRawTripPlanResult raw)
    {
        // إذا أعاد الـ AI أصفاراً، نقوم بالحساب برمجياً لضمان عدم ظهور 0 للمستخدم
        decimal stayTotal = raw.LodgingRecommendations.FirstOrDefault()?.Hotel.Price * (res.DurationDays - 1) ?? 0;
        decimal carTotal = raw.CarRecommendations.FirstOrDefault()?.Car.Price * res.DurationDays ?? 0;
        decimal activitiesTotal = raw.AttractionRecommendations.Sum(a => a.Attraction.Price);

        res.EstimatedCosts.Breakdown["stay"] = stayTotal;
        res.EstimatedCosts.Breakdown["car"] = carTotal;
        res.EstimatedCosts.Breakdown["activities"] = activitiesTotal;
        res.EstimatedCosts.GrandTotal = stayTotal + carTotal + activitiesTotal;
        res.EstimatedCosts.TotalPerDay = res.EstimatedCosts.GrandTotal / Math.Max(1, res.DurationDays);
    }
}