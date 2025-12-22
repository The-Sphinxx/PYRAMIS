using Agentic_Rentify.Application.Features.SystemSettings.Specifications;
using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Agentic_Rentify.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "System")]
public class SystemController(IUnitOfWork unitOfWork) : ControllerBase
{
    /// <summary>
    /// Get backgrounds for a page/section by pageName.
    /// </summary>
    /// <param name="pageName">Enum name from SystemPage.</param>
    /// <returns>Ordered list of backgrounds (ImageUrl + PublicId + DisplayOrder + Group).</returns>
    [HttpGet("backgrounds/{pageName}")]
    [ProducesResponseType(typeof(IEnumerable<SystemSetting>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBackgrounds([FromRoute] string pageName)
    {
        if (!Enum.TryParse<SystemPage>(pageName, ignoreCase: true, out var page))
        {
            return BadRequest(new { error = "Invalid pageName." });
        }

        var spec = new SystemSettingByPageSpecification(page);
        var list = await unitOfWork.Repository<SystemSetting>().ListAsync(spec);

        var ordered = list
            .OrderBy(s => s.DisplayOrder ?? int.MaxValue)
            .ToList();

        return Ok(ordered.Select(s => new
        {
            s.ImageUrl,
            s.PublicId,
            s.Group,
            s.DisplayOrder
        }));
    }
}
