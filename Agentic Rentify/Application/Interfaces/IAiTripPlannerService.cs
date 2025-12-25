using Agentic_Rentify.Application.Features.AiPlanner.DTOs;

namespace Agentic_Rentify.Application.Interfaces;

public interface IAiTripPlannerService
{
    Task<string> GenerateTripPlanAsync(
        string destination,
        DateTime startDate,
        DateTime endDate,
        string budgetLevel,
        int travelers,
        List<string> interests);
}
