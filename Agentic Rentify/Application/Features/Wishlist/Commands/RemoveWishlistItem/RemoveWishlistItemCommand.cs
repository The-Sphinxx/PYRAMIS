using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Core.Enums;
using Agentic_Rentify.Core.Exceptions;
using MediatR;

namespace Agentic_Rentify.Application.Features.Wishlist.Commands.RemoveWishlistItem;

public record RemoveWishlistItemCommand(string UserId, int ItemId, WishlistItemType ItemType) : IRequest<bool>;

public class RemoveWishlistItemCommandHandler : IRequestHandler<RemoveWishlistItemCommand, bool>
{
    private readonly IGenericRepository<WishlistItem> _wishlistRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveWishlistItemCommandHandler(IGenericRepository<WishlistItem> wishlistRepository, IUnitOfWork unitOfWork)
    {
        _wishlistRepository = wishlistRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RemoveWishlistItemCommand request, CancellationToken cancellationToken)
    {
        var existing = (await _wishlistRepository.GetAsync(w => w.UserId == request.UserId && w.ItemId == request.ItemId && w.ItemType == request.ItemType)).FirstOrDefault();
        if (existing == null)
        {
            throw new NotFoundException("Wishlist item not found");
        }

        await _wishlistRepository.DeleteAsync(existing);
        await _unitOfWork.CompleteAsync();
        return true;
    }
}
