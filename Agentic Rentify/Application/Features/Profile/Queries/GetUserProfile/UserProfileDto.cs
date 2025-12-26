using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Core.Enums;

namespace Agentic_Rentify.Application.Features.Profile.Queries.GetUserProfile;

public class UserProfileDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string ProfileImage { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Nationality { get; set; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string MembershipType { get; set; } = string.Empty;
    public List<BookingDto> Bookings { get; set; } = new();
    public List<WishlistItemDto> Wishlist { get; set; } = new();
}

public class WishlistItemDto
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public WishlistItemType ItemType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? Location { get; set; }
    public double? Rating { get; set; }
}
