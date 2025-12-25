using Agentic_Rentify.Application.Features.Trips.DTOs;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using AutoMapper;
using MediatR;
using Agentic_Rentify.Application.Features.SyncVector;

namespace Agentic_Rentify.Application.Features.Trips.Commands.UpdateTrip;

public class UpdateTripCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator) 
    : IRequestHandler<UpdateTripCommand, int>
{
    public async Task<int> Handle(UpdateTripCommand request, CancellationToken cancellationToken)
    {
        // Use specification to load all related data for update (replacing collections)
        var spec = new Specifications.TripByIdWithRelationsSpecification(request.Id);
        var trips = await unitOfWork.Repository<Trip>().ListAsync(spec);
        var trip = trips.FirstOrDefault();

        if (trip == null)
        {
            throw new Exception($"Trip with ID {request.Id} not found.");
        }

        // Clear existing collections to allow full replacement from DTO
        // This avoids duplication if the DTO sends items without IDs or creates new entities
        if (trip.Itinerary != null)
        {
            trip.Itinerary.Clear();
        }

        // Apply updates
        mapper.Map(request, trip);

        await unitOfWork.Repository<Trip>().UpdateAsync(trip);
        await unitOfWork.CompleteAsync();

        var text = string.Join(" ", new[] { trip.Title, trip.Description, trip.City });
        await mediator.Publish(new EntitySavedToVectorDbEvent(
            trip.Id,
            "Trip",
            text,
            name: trip.Title,
            price: trip.Price,
            city: trip.City), cancellationToken);

        return trip.Id;
    }
}
