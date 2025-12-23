using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Core.Exceptions;
using MediatR;

namespace Agentic_Rentify.Application.Features.Profile.Queries.GetUserProfile;

public record GetUserProfileQuery(string UserId) : IRequest<UserProfileDto>;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto>
{
    private readonly IIdentityService _identityService;
    private readonly IGenericRepository<Booking> _bookingRepository;
    private readonly IGenericRepository<WishlistItem> _wishlistRepository;

    public GetUserProfileQueryHandler(
        IIdentityService identityService,
        IGenericRepository<Booking> bookingRepository,
        IGenericRepository<WishlistItem> wishlistRepository)
    {
        _identityService = identityService;
        _bookingRepository = bookingRepository;
        _wishlistRepository = wishlistRepository;
    }

    public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _identityService.GetByIdAsync(request.UserId);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        var bookings = await _bookingRepository.GetAsync(b => b.UserId == request.UserId && !b.IsDeleted);
        var wishlist = await _wishlistRepository.GetAsync(w => w.UserId == request.UserId && !w.IsDeleted);

        var fullName = $"{user.FirstName} {user.LastName}".Trim();

        return new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = string.IsNullOrWhiteSpace(fullName) ? user.UserName ?? string.Empty : fullName,
            ProfileImage = user.ProfileImage ?? string.Empty,
            MembershipType = "Standard Member",
            Bookings = bookings.ToList(),
            Wishlist = wishlist
                .Select(w => new WishlistItemDto
                {
                    Id = w.Id,
                    ItemId = w.ItemId,
                    ItemType = w.ItemType,
                    Title = w.Title,
                    ImageUrl = w.ImageUrl,
                    Location = w.Location,
                    Rating = w.Rating
                })
                .ToList()
        };
    }
}
