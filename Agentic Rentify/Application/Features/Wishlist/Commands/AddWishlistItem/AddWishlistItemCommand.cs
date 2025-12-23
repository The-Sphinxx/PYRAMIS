using Agentic_Rentify.Application.Features.Profile.Queries.GetUserProfile;
using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Core.Enums;
using MediatR;

namespace Agentic_Rentify.Application.Features.Wishlist.Commands.AddWishlistItem;

public record AddWishlistItemCommand(
    string UserId,
    int ItemId,
    WishlistItemType ItemType,
    string Title,
    string? ImageUrl,
    string? Location,
    double? Rating
) : IRequest<WishlistItemDto>;

public class AddWishlistItemCommandHandler : IRequestHandler<AddWishlistItemCommand, WishlistItemDto>
{
    private readonly IGenericRepository<WishlistItem> _wishlistRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddWishlistItemCommandHandler(IGenericRepository<WishlistItem> wishlistRepository, IUnitOfWork unitOfWork)
    {
        _wishlistRepository = wishlistRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<WishlistItemDto> Handle(AddWishlistItemCommand request, CancellationToken cancellationToken)
    {
        var existing = (await _wishlistRepository.GetAsync(w => w.UserId == request.UserId && w.ItemId == request.ItemId && w.ItemType == request.ItemType)).FirstOrDefault();
        if (existing != null)
        {
            return Map(existing);
        }

        var entity = new WishlistItem
        {
            UserId = request.UserId,
            ItemId = request.ItemId,
            ItemType = request.ItemType,
            Title = request.Title,
            ImageUrl = request.ImageUrl,
            Location = request.Location,
            Rating = request.Rating
        };

        await _wishlistRepository.AddAsync(entity);
        await _unitOfWork.CompleteAsync();
        return Map(entity);
    }

    private static WishlistItemDto Map(WishlistItem item) => new WishlistItemDto
    {
        Id = item.Id,
        ItemId = item.ItemId,
        ItemType = item.ItemType,
        Title = item.Title,
        ImageUrl = item.ImageUrl,
        Location = item.Location,
        Rating = item.Rating
    };
}
