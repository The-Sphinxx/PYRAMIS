using Agentic_Rentify.Application.Features.Profile.Queries.GetUserProfile;
using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Entities;
using MediatR;

namespace Agentic_Rentify.Application.Features.Wishlist.Queries.GetWishlistByUser;

public record GetWishlistByUserQuery(string UserId) : IRequest<List<WishlistItemDto>>;

public class GetWishlistByUserQueryHandler : IRequestHandler<GetWishlistByUserQuery, List<WishlistItemDto>>
{
    private readonly IGenericRepository<WishlistItem> _wishlistRepository;

    public GetWishlistByUserQueryHandler(IGenericRepository<WishlistItem> wishlistRepository)
    {
        _wishlistRepository = wishlistRepository;
    }

    public async Task<List<WishlistItemDto>> Handle(GetWishlistByUserQuery request, CancellationToken cancellationToken)
    {
        var items = await _wishlistRepository.GetAsync(w => w.UserId == request.UserId && !w.IsDeleted);
        return items
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
            .ToList();
    }
}
