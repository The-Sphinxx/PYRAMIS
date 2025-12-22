using Agentic_Rentify.Core.Common;
using Agentic_Rentify.Core.Enums;

namespace Agentic_Rentify.Core.Entities;

public class SystemSetting : BaseEntity
{
    public SystemPage PageName { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string PublicId { get; set; } = string.Empty;
    public string? Group { get; set; }
    public int? DisplayOrder { get; set; }
}
