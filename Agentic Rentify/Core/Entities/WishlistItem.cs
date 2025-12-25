using Agentic_Rentify.Core.Common;
using Agentic_Rentify.Core.Enums;

namespace Agentic_Rentify.Core.Entities;

public class WishlistItem : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public WishlistItemType ItemType { get; set; }

    // Denormalized summary to avoid extra joins when listing wishlist
    public string Title { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? Location { get; set; }
    public double? Rating { get; set; }
}
