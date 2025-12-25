using Agentic_Rentify.Application.Features.Attractions.DTOs;
using MediatR;

namespace Agentic_Rentify.Application.Features.Attractions.Commands.PatchAttraction;

public class PatchAttractionCommand : PatchAttractionDTO, IRequest<int>
{
    public int Id { get; set; }

    public PatchAttractionCommand(int id)
    {
        Id = id;
    }
}
