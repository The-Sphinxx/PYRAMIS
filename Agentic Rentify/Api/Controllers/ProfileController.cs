using Agentic_Rentify.Application.Features.Profile.Queries.GetUserProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Agentic_Rentify.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<UserProfileDto>> GetCurrent()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var result = await mediator.Send(new GetUserProfileQuery(userId));
        return Ok(result);
    }

    [HttpGet("{userId}")]
    [AllowAnonymous]
    public async Task<ActionResult<UserProfileDto>> GetById(string userId)
    {
        var result = await mediator.Send(new GetUserProfileQuery(userId));
        return Ok(result);
    }
}
