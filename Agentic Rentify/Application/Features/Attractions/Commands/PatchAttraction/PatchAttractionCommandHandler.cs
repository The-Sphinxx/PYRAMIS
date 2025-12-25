using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Entities;
using MediatR;

namespace Agentic_Rentify.Application.Features.Attractions.Commands.PatchAttraction;

public class PatchAttractionCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<PatchAttractionCommand, int>
{
    public async Task<int> Handle(PatchAttractionCommand request, CancellationToken cancellationToken)
    {
        var attraction = await unitOfWork.Repository<Attraction>().GetByIdAsync(request.Id);

        if (attraction == null) throw new KeyNotFoundException($"Attraction with ID {request.Id} not found.");

        if (request.Status != null) attraction.Status = request.Status;
        if (request.Availability != null) attraction.Availability = request.Availability;
        if (request.IsFeatured != null) attraction.IsFeatured = request.IsFeatured.Value;

        await unitOfWork.Repository<Attraction>().UpdateAsync(attraction);
        await unitOfWork.CompleteAsync();
        
        return attraction.Id;
    }
}
