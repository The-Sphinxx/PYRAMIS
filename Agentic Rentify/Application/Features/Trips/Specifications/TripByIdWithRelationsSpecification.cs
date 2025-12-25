using System.Linq.Expressions;
using Agentic_Rentify.Application.Specifications;
using Agentic_Rentify.Core.Entities;

namespace Agentic_Rentify.Application.Features.Trips.Specifications;

public sealed class TripByIdWithRelationsSpecification : BaseSpecification<Trip>
{
    public TripByIdWithRelationsSpecification(int id)
        : base(trip => trip.Id == id)
    {
        AddInclude(t => t.UserReviews);
        AddInclude(t => t.Itinerary);
        // If Itinerary has nested collections, we might need string includes if ThenInclude isn't wrapped
        // But for now assuming ItineraryDay is the main blocker or owned.
        AddInclude("Itinerary.Activities");
    }
}
