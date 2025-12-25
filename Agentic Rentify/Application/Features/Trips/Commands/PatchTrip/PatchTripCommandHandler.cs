using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using MediatR;

namespace Agentic_Rentify.Application.Features.Trips.Commands.PatchTrip;

public class PatchTripCommandHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<PatchTripCommand, int>
{
    public async Task<int> Handle(PatchTripCommand request, CancellationToken cancellationToken)
    {
        var trip = await unitOfWork.Repository<Trip>().GetByIdAsync(request.Id);
        if (trip == null)
        {
            throw new Exception($"Trip with ID {request.Id} not found.");
        }

        // Apply partial updates
        if (request.Status != null)
        {
            trip.Status = request.Status;
        }

        if (request.Featured.HasValue)
        {
            trip.Featured = request.Featured.Value;
            trip.IsFeatured = request.Featured.Value;
        }

        if (request.IsFeatured.HasValue)
        {
            trip.IsFeatured = request.IsFeatured.Value;
            trip.Featured = request.IsFeatured.Value;
        }

        await unitOfWork.Repository<Trip>().UpdateAsync(trip);
        await unitOfWork.CompleteAsync();

        return trip.Id;
    }
}
