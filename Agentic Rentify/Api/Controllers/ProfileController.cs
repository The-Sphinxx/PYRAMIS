using Agentic_Rentify.Application.Features.Profile.Queries.GetUserProfile;
using Agentic_Rentify.Application.Features.Profile.Commands.UpdateProfile;
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

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        command.UserId = userId;
        await mediator.Send(command);
        return Ok(new { message = "Profile updated successfully" });
    }
}
