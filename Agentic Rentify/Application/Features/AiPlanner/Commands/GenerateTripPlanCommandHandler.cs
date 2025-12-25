using Agentic_Rentify.Application.Features.AiPlanner.DTOs;
using Agentic_Rentify.Application.Interfaces;
using MediatR;
using System.Text.Json;

namespace Agentic_Rentify.Application.Features.AiPlanner.Commands.GenerateTripPlan;

public class GenerateTripPlanCommandHandler(IAiTripPlannerService aiTripPlannerService)
    : IRequestHandler<GenerateTripPlanCommand, TripPlanResponse>
{
    private readonly IAiTripPlannerService _aiTripPlannerService = aiTripPlannerService;

    public async Task<TripPlanResponse> Handle(
        GenerateTripPlanCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Criteria.Destination))
            {
                return new TripPlanResponse
                {
                    Success = false,
                    ErrorMessage = "Destination is required"
                };
            }

            if (request.Criteria.StartDate >= request.Criteria.EndDate)
            {
                return new TripPlanResponse
                {
                    Success = false,
                    ErrorMessage = "End date must be after start date"
                };
            }

            if (request.Criteria.Travelers < 1)
            {
                return new TripPlanResponse
                {
                    Success = false,
                    ErrorMessage = "At least one traveler is required"
                };
            }

            // Generate trip plan using AI
            var aiResultJson = await _aiTripPlannerService.GenerateTripPlanAsync(
                request.Criteria.Destination,
                request.Criteria.StartDate,
                request.Criteria.EndDate,
                request.Criteria.BudgetLevel,
                request.Criteria.Travelers,
                request.Criteria.Interests
            );

            // Parse AI response
            TripPlanResponse? tripPlan;
            try
            {
                // Try to parse as TripPlanResponse directly
                tripPlan = JsonSerializer.Deserialize<TripPlanResponse>(aiResultJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                });

                if (tripPlan == null)
                {
                    return new TripPlanResponse
                    {
                        Success = false,
                        ErrorMessage = "Failed to parse AI response"
                    };
                }

                tripPlan.Success = true;
                return tripPlan;
            }
            catch (JsonException)
            {
                // If parsing fails, create a structured error response
                return new TripPlanResponse
                {
                    Success = false,
                    ErrorMessage = "AI generated invalid response format. Raw response: " + aiResultJson.Substring(0, Math.Min(500, aiResultJson.Length)),
                    TripOverview = new TripOverview
                    {
                        Destination = request.Criteria.Destination,
                        StartDate = request.Criteria.StartDate.ToString("yyyy-MM-dd"),
                        EndDate = request.Criteria.EndDate.ToString("yyyy-MM-dd"),
                        DurationDays = (request.Criteria.EndDate - request.Criteria.StartDate).Days + 1,
                        Travelers = request.Criteria.Travelers,
                        BudgetLevel = request.Criteria.BudgetLevel
                    }
                };
            }
        }
        catch (Exception ex)
        {
            return new TripPlanResponse
            {
                Success = false,
                ErrorMessage = $"Error generating trip plan: {ex.Message}"
            };
        }
    }
}
