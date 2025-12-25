using Agentic_Rentify.Application.Features.Cars.DTOs;
using MediatR;

namespace Agentic_Rentify.Application.Features.Cars.Commands.PatchCar;

public class PatchCarCommand : PatchCarDTO, IRequest<int>
{
    public int Id { get; set; }

    public PatchCarCommand(int id)
    {
        Id = id;
    }
}
