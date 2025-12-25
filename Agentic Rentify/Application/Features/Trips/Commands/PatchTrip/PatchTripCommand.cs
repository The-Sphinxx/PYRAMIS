using Agentic_Rentify.Application.Features.Trips.DTOs;
using MediatR;

namespace Agentic_Rentify.Application.Features.Trips.Commands.PatchTrip;

public class PatchTripCommand : PatchTripDTO, IRequest<int>
{
    public int Id { get; set; }

    public PatchTripCommand(int id)
    {
        Id = id;
    }
}
