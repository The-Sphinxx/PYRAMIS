using Agentic_Rentify.Application.Features.Hotels.DTOs;
using MediatR;

namespace Agentic_Rentify.Application.Features.Hotels.Commands.PatchHotel;

public class PatchHotelCommand : PatchHotelDTO, IRequest<int>
{
    public int Id { get; set; }

    public PatchHotelCommand(int id)
    {
        Id = id;
    }
}