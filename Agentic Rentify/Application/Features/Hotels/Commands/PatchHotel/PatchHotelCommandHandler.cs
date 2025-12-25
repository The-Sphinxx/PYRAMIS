using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using MediatR;

namespace Agentic_Rentify.Application.Features.Hotels.Commands.PatchHotel;

public class PatchHotelCommandHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<PatchHotelCommand, int>
{
    public async Task<int> Handle(PatchHotelCommand request, CancellationToken cancellationToken)
    {
        var hotel = await unitOfWork.Repository<Hotel>().GetByIdAsync(request.Id);
        if (hotel == null)
        {
            throw new Exception($"Hotel with ID {request.Id} not found.");
        }

        // Apply partial updates
        if (request.Status != null)
        {
            hotel.Status = request.Status;
        }

        if (request.Featured.HasValue)
        {
            hotel.Featured = request.Featured.Value;
        }

        if (request.IsFeatured.HasValue)
        {
            hotel.IsFeatured = request.IsFeatured.Value;
        }

        await unitOfWork.Repository<Hotel>().UpdateAsync(hotel);
        await unitOfWork.CompleteAsync();

        return hotel.Id;
    }
}