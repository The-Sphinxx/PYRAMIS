using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Application.Features.SyncVector;
using MediatR;

namespace Agentic_Rentify.Application.Features.Trips.Commands.DeleteTrip;

public class DeleteTripCommandHandler(IUnitOfWork unitOfWork, IMediator mediator) 
    : IRequestHandler<DeleteTripCommand, int>
{
    public async Task<int> Handle(DeleteTripCommand request, CancellationToken cancellationToken)
    {
        var spec = new Specifications.TripByIdWithRelationsSpecification(request.Id);
        var trips = await unitOfWork.Repository<Trip>().ListAsync(spec);
        var trip = trips.FirstOrDefault();

        if (trip == null)
        {
            throw new Exception($"Trip with ID {request.Id} not found.");
        }

        // Manually clear related entities to enforce cascade delete behavior
        // logic: Remove them from collection -> EF Core marks as deleted if configured as Composition
        if (trip.UserReviews != null)
        {
            trip.UserReviews.Clear();
        }

        if (trip.Itinerary != null)
        {
            trip.Itinerary.Clear();
        }

        // We update first to let EF Core delete the orphans
        await unitOfWork.Repository<Trip>().UpdateAsync(trip);
        await unitOfWork.CompleteAsync();

        // Now delete the trip itself
        await unitOfWork.Repository<Trip>().DeleteAsync(trip);
        await unitOfWork.CompleteAsync();

        await mediator.Publish(new EntityDeletedFromVectorDbEvent(trip.Id, "Trip"), cancellationToken);

        return trip.Id;
    }
}

