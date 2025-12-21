using Agentic_Rentify.Infragentic.Services;
using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Application.Features.Photos.Commands.DeletePhoto;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
public class AdminController(DataSyncService dataSyncService, IUnitOfWork unitOfWork, IPhotoService photoService, IMediator mediator) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPhotoService _photoService = photoService;
    private readonly IMediator _mediator = mediator;
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
}
