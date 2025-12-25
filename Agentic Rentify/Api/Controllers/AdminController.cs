using Agentic_Rentify.Infragentic.Services;
using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Application.Features.Photos.Commands.DeletePhoto;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Core.Enums;
using Agentic_Rentify.Application.Features.Admin.Commands.CreateUser;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Agentic_Rentify.Api.Controllers;

/// <summary>
/// Administrative operations and system management
/// </summary>
/// <remarks>
/// Provides endpoints for system maintenance and data synchronization.
/// Should be protected with admin-only authorization in production.
/// </remarks>
[ApiController]
[Route("api/admin")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "Admin")]
public class AdminController(DataSyncService dataSyncService, IUnitOfWork unitOfWork, IPhotoService photoService, IMediator mediator, UserManager<ApplicationUser> userManager) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPhotoService _photoService = photoService;
    private readonly IMediator _mediator = mediator;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    /// <summary>
    /// Manually trigger vector database synchronization for all entities.
    /// </summary>
    /// <returns>Sync completion status</returns>
    /// <remarks>
    /// This endpoint forces a complete re-sync of all entities (Trips, Attractions, Hotels, Cars) to the Qdrant vector database.
    /// 
    /// **Use Cases:**
    /// - After database restore or migration
    /// - When vector search results seem outdated
    /// - Initial setup after deployment
    /// 
    /// **Note:** This operation is also performed automatically at application startup.
    /// The sync will embed all entity descriptions and update the search index.
    /// </remarks>
    /// <response code="200">Synchronization completed successfully</response>
    [HttpGet("vector-sync")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> VectorSync()
    {
        await dataSyncService.SyncAsync("rentify_memory");
        return Ok(new { status = "ok", message = "Vector database synchronization completed" });
    }


    /// <summary>
    /// Upload or update the background image for a specific system page.
    /// </summary>
    /// <param name="page">Target page: Home, Login, or Signup.</param>
    /// <param name="file">Image file to upload.</param>
    /// <returns>Updated SystemSetting with new image URL and PublicId.</returns>
    [HttpPost("update-background")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SystemSetting), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateBackground([FromQuery] SystemPage page, IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new { error = "File cannot be null or empty." });
        }

        // Get existing setting
        var spec = new Agentic_Rentify.Application.Features.SystemSettings.Specifications.SystemSettingByPageSpecification(page);
        var existing = (await _unitOfWork.Repository<SystemSetting>().ListAsync(spec)).FirstOrDefault();
        var oldPublicId = existing?.PublicId ?? string.Empty;

        // Upload new image
        var upload = await _photoService.AddPhotoAsync(file);

        if (existing is null)
        {
            existing = new SystemSetting
            {
                PageName = page,
                ImageUrl = upload.Url,
                PublicId = upload.PublicId
            };
            await _unitOfWork.Repository<SystemSetting>().AddAsync(existing);
        }
        else
        {
            existing.ImageUrl = upload.Url;
            existing.PublicId = upload.PublicId;
            existing.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Repository<SystemSetting>().UpdateAsync(existing);
        }

        await _unitOfWork.CompleteAsync();

        // Delete old image via CQRS if applicable
        if (!string.IsNullOrWhiteSpace(oldPublicId) && oldPublicId != upload.PublicId)
        {
            await _mediator.Send(new DeletePhotoCommand { PublicId = oldPublicId });
        }

        return Ok(existing);
    }

    /// <summary>
    /// Upload multiple background images for a page (supports carousels).
    /// </summary>
    /// <param name="page">Target page.</param>
    /// <param name="group">Optional grouping key (e.g., "default").</param>
    /// <param name="files">Images to upload.</param>
    /// <returns>List of created settings.</returns>
    [HttpPost("update-backgrounds")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<SystemSetting>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateBackgrounds([FromQuery] SystemPage page, [FromQuery] string? group, [FromForm] List<IFormFile> files)
    {
        if (files is null || files.Count == 0)
        {
            return BadRequest(new { error = "No files provided." });
        }

        var created = new List<SystemSetting>();
        int order = 1;

        foreach (var file in files)
        {
            var upload = await _photoService.AddPhotoAsync(file);
            var setting = new SystemSetting
            {
                PageName = page,
                ImageUrl = upload.Url,
                PublicId = upload.PublicId,
                Group = group,
                DisplayOrder = order++,
            };
            await _unitOfWork.Repository<SystemSetting>().AddAsync(setting);
            created.Add(setting);
        }

        await _unitOfWork.CompleteAsync();
        return Ok(created.Select(s => new { s.Id, s.ImageUrl, s.PublicId, s.Group, s.DisplayOrder }));
    }

    [HttpGet("users")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers(int pageNumber = 1, int pageSize = 10000)
    {
        var users = _userManager.Users
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var usersWithRoles = new List<object>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            usersWithRoles.Add(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.FirstName,
                user.LastName,
                FullName = $"{user.FirstName} {user.LastName}",
                Phone = user.PhoneNumber,
                user.Nationality,
                user.ProfileImage,
                user.IsVerified,
                Status = user.LockoutEnd > DateTimeOffset.UtcNow ? "Suspended" : (user.IsVerified ? "Verified" : "Active"),
                Role = roles.FirstOrDefault() ?? "Client",
                user.CreatedAt
            });
        }

        return Ok(new {
            pageNumber,
            pageSize,
            totalRecords = _userManager.Users.Count(),
            totalPages = (int)Math.Ceiling(_userManager.Users.Count() / (double)pageSize),
            data = usersWithRoles
        });
    }

    /// <summary>
    /// Get a single user by ID.
    /// </summary>
    /// <param name="id">User ID (GUID)</param>
    /// <returns>User details</returns>
    [HttpGet("users/{id}")]
    [ProducesResponseType(typeof(ApplicationUser), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { error = $"User with ID {id} not found." });
        }

        return Ok(user);
    }

    /// <summary>
    /// Create a new user.
    /// </summary>
    /// <param name="request">User creation data</param>
    /// <returns>Created user ID</returns>
    [HttpPost("users")]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var command = new CreateUserCommand
        {
            Email = request.Email,
            Password = request.Password,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            Nationality = request.Nationality,
            ProfileImage = request.ProfileImage,
            Role = request.Role
        };

        var userId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetUser), new { id = userId }, userId);
    }

    /// <summary>
    /// Update user details (full update).
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">Updated user data</param>
    /// <returns>Success response</returns>
    [HttpPut("users/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequest request)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { error = $"User with ID {id} not found." });
        }

        // Update properties
        user.FirstName = request.FirstName ?? user.FirstName;
        user.LastName = request.LastName ?? user.LastName;
        user.Email = request.Email ?? user.Email;
        user.UserName = request.Email ?? user.UserName;
        user.PhoneNumber = request.Phone ?? user.PhoneNumber;
        user.Nationality = request.Nationality ?? user.Nationality;
        user.ProfileImage = request.ProfileImage ?? user.ProfileImage;

        var result = await _userManager.UpdateAsync(user);
        
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }

        return Ok(new { message = "User updated successfully.", user });
    }

    /// <summary>
    /// Partially update user (PATCH).
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">Partial update data</param>
    /// <returns>Success response</returns>
    [HttpPatch("users/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PatchUser(string id, [FromBody] PatchUserRequest request)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { error = $"User with ID {id} not found." });
        }

        // Update only provided fields
        if (request.FirstName != null) user.FirstName = request.FirstName;
        if (request.LastName != null) user.LastName = request.LastName;
        if (request.Email != null)
        {
            user.Email = request.Email;
            user.UserName = request.Email;
        }
        if (request.Phone != null) user.PhoneNumber = request.Phone;
        if (request.Nationality != null) user.Nationality = request.Nationality;
        if (request.ProfileImage != null) user.ProfileImage = request.ProfileImage;
        if (request.Status != null) 
        {
            if (request.Status == "Suspended")
            {
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.MaxValue;
                user.IsVerified = false; // Optional: mark as unverified if suspended
            }
            else
            {
                // Clear lockout if status is not Suspended
                user.LockoutEnd = null;
                user.IsVerified = request.Status == "Verified";
            }
        }

        var result = await _userManager.UpdateAsync(user);
        
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }

        return Ok(new { message = "User updated successfully.", user });
    }

    /// <summary>
    /// Delete a user.
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Success response</returns>
    [HttpDelete("users/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { error = $"User with ID {id} not found." });
        }

        var result = await _userManager.DeleteAsync(user);
        
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }

        return Ok(new { message = "User deleted successfully." });
    }

}

// ==================== DTOs ====================
public record CreateUserRequest
{
    public string Email { get; init; } = string.Empty;
    public string? Password { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Phone { get; init; }
    public string? Nationality { get; init; }
    public string? ProfileImage { get; init; }
    public string? Role { get; init; }
}

public record UpdateUserRequest
{
    public string? Email { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Phone { get; init; }
    public string? Nationality { get; init; }
    public string? ProfileImage { get; init; }
}

public record PatchUserRequest
{
    public string? Email { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Phone { get; init; }
    public string? Nationality { get; init; }
    public string? ProfileImage { get; init; }
    public string? Status { get; init; }
}
