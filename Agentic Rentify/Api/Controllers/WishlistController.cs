using Agentic_Rentify.Application.Features.Wishlist.Commands.AddWishlistItem;
using Agentic_Rentify.Application.Features.Wishlist.Commands.RemoveWishlistItem;
using Agentic_Rentify.Application.Features.Wishlist.Queries.GetWishlistByUser;
using Agentic_Rentify.Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Agentic_Rentify.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WishlistController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var result = await mediator.Send(new GetWishlistByUserQuery(userId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddWishlistItemCommand command)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var corrected = command with { UserId = userId };
        var result = await mediator.Send(corrected);
        return Ok(result);
    }

    [HttpDelete("{itemType}/{itemId}")]
    public async Task<IActionResult> Remove(string itemType, int itemId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        if (!Enum.TryParse<WishlistItemType>(itemType, true, out var parsed))
        {
            return BadRequest(new { error = "Invalid item type" });
        }

        await mediator.Send(new RemoveWishlistItemCommand(userId, itemId, parsed));
        return NoContent();
    }
}
