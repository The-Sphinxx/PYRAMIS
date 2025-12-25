using Agentic_Rentify.Application.Features.AiPlanner.DTOs;
using MediatR;

namespace Agentic_Rentify.Application.Features.AiPlanner.Commands.GenerateTripPlan;

public record GenerateTripPlanCommand(TripSearchCriteria Criteria) : IRequest<TripPlanResponse>;
