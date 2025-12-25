using Agentic_Rentify.Application.Features.AiPlanner.Commands.GenerateTripPlan;
using Agentic_Rentify.Application.Features.AiPlanner.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agentic_Rentify.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiPlannerController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Generate a personalized AI trip plan based on user criteria
    /// </summary>
    /// <param name="criteria">Trip search criteria including destination, dates, budget, and interests</param>
    /// <returns>A comprehensive trip plan with itinerary, costs, and recommendations</returns>
    [HttpPost("generate")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TripPlanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GenerateTripPlan([FromBody] TripSearchCriteria criteria)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var command = new GenerateTripPlanCommand(criteria);
        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get available Egyptian cities for trip planning
    /// </summary>
    [HttpGet("destinations")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<DestinationInfo>), StatusCodes.Status200OK)]
    public IActionResult GetDestinations()
    {
        var destinations = new List<DestinationInfo>
        {
            new() { Name = "Cairo", DisplayName = "Cairo - القاهرة", Region = "Greater Cairo", Popular = true },
            new() { Name = "Alexandria", DisplayName = "Alexandria - الإسكندرية", Region = "Mediterranean", Popular = true },
            new() { Name = "Luxor", DisplayName = "Luxor - الأقصر", Region = "Upper Egypt", Popular = true },
            new() { Name = "Aswan", DisplayName = "Aswan - أسوان", Region = "Upper Egypt", Popular = true },
            new() { Name = "Sharm El Sheikh", DisplayName = "Sharm El Sheikh - شرم الشيخ", Region = "Sinai", Popular = true },
            new() { Name = "Hurghada", DisplayName = "Hurghada - الغردقة", Region = "Red Sea", Popular = true },
            new() { Name = "Dahab", DisplayName = "Dahab - دهب", Region = "Sinai", Popular = false },
            new() { Name = "Marsa Alam", DisplayName = "Marsa Alam - مرسى علم", Region = "Red Sea", Popular = false },
            new() { Name = "Siwa", DisplayName = "Siwa - سيوة", Region = "Western Desert", Popular = false },
            new() { Name = "El Gouna", DisplayName = "El Gouna - الجونة", Region = "Red Sea", Popular = false },
            new() { Name = "Fayoum", DisplayName = "Fayoum - الفيوم", Region = "Middle Egypt", Popular = false },
            new() { Name = "Port Said", DisplayName = "Port Said - بورسعيد", Region = "Canal Zone", Popular = false }
        };

        return Ok(destinations);
    }

    /// <summary>
    /// Get available interest categories for trip customization
    /// </summary>
    [HttpGet("interests")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<InterestCategory>), StatusCodes.Status200OK)]
    public IActionResult GetInterests()
    {
        var interests = new List<InterestCategory>
        {
            new() { Id = "history", Name = "History & Culture", Icon = "🏛️" },
            new() { Id = "beach", Name = "Beach & Relaxation", Icon = "🏖️" },
            new() { Id = "adventure", Name = "Adventure & Sports", Icon = "🏃" },
            new() { Id = "diving", Name = "Diving & Snorkeling", Icon = "🤿" },
            new() { Id = "museums", Name = "Museums & Art", Icon = "🎨" },
            new() { Id = "food", Name = "Food & Cuisine", Icon = "🍽️" },
            new() { Id = "shopping", Name = "Shopping & Markets", Icon = "🛍️" },
            new() { Id = "nightlife", Name = "Nightlife & Entertainment", Icon = "🎭" },
            new() { Id = "nature", Name = "Nature & Wildlife", Icon = "🌳" },
            new() { Id = "photography", Name = "Photography", Icon = "📸" }
        };

        return Ok(interests);
    }
}

public class DestinationInfo
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public bool Popular { get; set; }
}

public class InterestCategory
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}
